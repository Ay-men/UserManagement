using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Polly;
using Polly.CircuitBreaker;
using RabbitMQ.Client;

public class RabbitMQService
{
  private readonly IConfiguration _configuration;
  private readonly ConnectionFactory _factory;
  private IConnection _connection;
  private IModel _channel;
  private readonly AsyncCircuitBreakerPolicy _circuitBreaker;

  public RabbitMQService(IConfiguration configuration)
  {
    _configuration = configuration;
    _factory = new ConnectionFactory()
    {
      HostName = _configuration["RabbitMQ:HostName"],
      UserName = _configuration["RabbitMQ:UserName"],
      Password = _configuration["RabbitMQ:Password"],
      Port = int.Parse(_configuration["RabbitMQ:Port"])
    };

    _circuitBreaker = Policy
        .Handle<Exception>()
        .CircuitBreakerAsync(
            exceptionsAllowedBeforeBreaking: 3,
            durationOfBreak: TimeSpan.FromSeconds(30)
        );

    InitializeRabbitMQ();
  }

  private void InitializeRabbitMQ()
  {
    _connection = _factory.CreateConnection();
    _channel = _connection.CreateModel();

    _channel.ExchangeDeclare(exchange: "user_events", type: ExchangeType.Topic, durable: true);
    _channel.QueueDeclare(queue: "user_created_queue", durable: true, exclusive: false, autoDelete: false);
    _channel.QueueBind(queue: "user_created_queue", exchange: "user_events", routingKey: "user.created");
  }

  public async Task PublishEventAsync<T>(string exchangeName, string routingKey, T message)
  {
    await _circuitBreaker.ExecuteAsync(async () =>
    {
      if (_channel == null || _channel.IsClosed)
      {
        InitializeRabbitMQ();
      }

      var json = JsonSerializer.Serialize(message);
      var body = Encoding.UTF8.GetBytes(json);

      var properties = _channel.CreateBasicProperties();
      properties.Persistent = true;

      _channel.BasicPublish(exchange: exchangeName,
                                routingKey: routingKey,
                                basicProperties: properties,
                                body: body);
    });
  }

  public void Dispose()
  {
    _channel?.Close();
    _connection?.Close();
  }
}