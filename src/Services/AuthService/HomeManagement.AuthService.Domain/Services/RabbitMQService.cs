using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Polly;
using Polly.CircuitBreaker;
using RabbitMQ.Client;

namespace HomeManagement.AuthService.Domain.Services
{
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
      _factory = new ConnectionFactory() { HostName = _configuration["RabbitMQ:HostName"] };

      // Circuit Breaker implementation
      _circuitBreaker = Policy
          .Handle<Exception>()
          .CircuitBreakerAsync(
              exceptionsAllowedBeforeBreaking: 3,
              durationOfBreak: TimeSpan.FromSeconds(30)
          );
    }

    public async Task PublishEventAsync<T>(string exchangeName, string routingKey, T message)
    {
      await _circuitBreaker.ExecuteAsync(async () =>
      {
        // Circuit Breaker: This method will be protected by the circuit breaker.
        // If it fails 3 times in a row, the circuit will open for 30 seconds.
        if (_connection == null || !_connection.IsOpen)
        {
          _connection = _factory.CreateConnection();
        }

        if (_channel == null || _channel.IsClosed)
        {
          _channel = _connection.CreateModel();
        }

        _channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic);

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        _channel.BasicPublish(exchange: exchangeName,
                                    routingKey: routingKey,
                                    basicProperties: null,
                                    body: body);
      });
    }

    // Remember to implement IDisposable and close connections in Dispose method
  }
}