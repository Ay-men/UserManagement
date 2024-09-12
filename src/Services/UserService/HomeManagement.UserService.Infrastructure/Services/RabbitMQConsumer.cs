using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using MediatR;
using HomeManagement.Shared;

namespace HomeManagement.UserService.Infrastructure.Services
{
  public class RabbitMQConsumer : BackgroundService
  {
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public RabbitMQConsumer(IConfiguration configuration, IServiceProvider serviceProvider)
    {
      _configuration = configuration;
      _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672, UserName = "guest", Password = "guest" };
      using var connection = factory.CreateConnection();
      using var channel = connection.CreateModel();

      channel.QueueDeclare(queue: "user_created_queue",
                          durable: false,
                          exclusive: false,
                          autoDelete: false,
                          arguments: null);

      channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

      var consumer = new EventingBasicConsumer(channel);
      consumer.Received += async (model, ea) =>
      {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        var userCreatedEvent = JsonConvert.DeserializeObject<UserCreatedEvent>(message);

        using (var scope = _serviceProvider.CreateScope())
        {
          var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
          await mediator.Publish(userCreatedEvent);
        }

        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
      };

      channel.BasicConsume(queue: "user_created_queue",
                          autoAck: false,
                          consumer: consumer);

      await Task.CompletedTask;
    }
  }
}