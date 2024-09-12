using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using HomeManagement.UserService.Application.Commands;

namespace HomeManagement.UserService.Infrastructure.Services;

public class RabbitMQConsumer : BackgroundService
{
  private readonly IConnection _connection;
  private readonly IModel _channel;
  private readonly ILogger<RabbitMQConsumer> _logger;
  private readonly RabbitMQSettings _settings;
  private readonly IServiceProvider _serviceProvider;
  private readonly IMediator _mediator;
  private readonly IServiceScopeFactory _serviceScopeFactory;

  public RabbitMQConsumer(
      IOptions<RabbitMQSettings> settings,
      ILogger<RabbitMQConsumer> logger,
      IServiceProvider serviceProvider,
       IMediator mediator,
        IServiceScopeFactory serviceScopeFactory)
  {
    _logger = logger;
    _settings = settings.Value;
    _serviceProvider = serviceProvider;
    _mediator = mediator;
    _serviceScopeFactory = serviceScopeFactory;


    var factory = new ConnectionFactory()
    {
      HostName = "localhost",
      UserName = "guest",
      Password = "guest"
    };

    _connection = factory.CreateConnection();
    _channel = _connection.CreateModel();
    _channel.QueueDeclare("user_created_queue", durable: true, exclusive: false, autoDelete: false);
    _channel.BasicQos(0, 1, false);
  }

  protected override Task ExecuteAsync(CancellationToken stoppingToken)
  {
    var consumer = new EventingBasicConsumer(_channel);
    consumer.Received += async (model, ea) =>
    {
      var body = ea.Body.ToArray();
      var message = Encoding.UTF8.GetString(body);
      _logger.LogInformation($"Received message: {message}");

      try
      {
        await ProcessMessageAsync(message);
        _channel.BasicAck(ea.DeliveryTag, false);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error processing message");
        _channel.BasicNack(ea.DeliveryTag, false, true);
      }
    };

    _channel.BasicConsume("user_created_queue", false, consumer);
    return Task.CompletedTask;
  }

  private async Task ProcessMessageAsync(string message, CancellationToken cancellationToken = default)
  {
    using (var scope = _serviceScopeFactory.CreateScope())
    {
      var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
      var userMessage = JsonSerializer.Deserialize<UserMessage>(message);
      if (userMessage?.Email == null)
        return;
      var userId = Guid.NewGuid();
      var command = new CreateUserProfileCommand
      {
        UserId = Guid.NewGuid(),
        Email = userMessage.Email,
        FirstName = userMessage.FirstName,
        LastName = userMessage.LastName
      };
      var userIdw = await mediator.Send(command, cancellationToken);
      _logger.LogInformation($"Created new user profile with ID: {userIdw}");
    }





  }

  public override void Dispose()
  {
    _channel?.Close();
    _connection?.Close();
    base.Dispose();
  }
}

public class RabbitMQSettings
{
  public string HostName { get; set; }
  public string UserName { get; set; }
  public string Password { get; set; }
  public string QueueName { get; set; }
}

public class UserMessage
{
  public string UserName { get; set; }
  public string Email { get; set; }
  public string FirstName { get; set; }
  public string LastName { get; set; }
}
