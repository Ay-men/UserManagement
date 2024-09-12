using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Polly;
using Polly.Retry;
using System;
using System.Threading.Tasks;
using System.Net.Sockets;
using RabbitMQ.Client.Events;

namespace HomeManagement.Shared.RabbitMQ
{
  public class RabbitMQService : IRabbitMQService
  {
    private readonly ConnectionFactory _factory;
    private readonly IModel _channel;
    private readonly AsyncRetryPolicy _retryPolicy;

    public RabbitMQService(IConfiguration configuration)
    {
      _factory = new ConnectionFactory
      {
        HostName = configuration["RabbitMQ:HostName"],
        UserName = configuration["RabbitMQ:UserName"],
        Password = configuration["RabbitMQ:Password"],
        Port = int.Parse(configuration["RabbitMQ:Port"])
      };

      _retryPolicy = Policy.Handle<BrokerUnreachableException>()
                           .Or<SocketException>()
                           .RetryAsync(3, onRetry: (exception, retryCount) =>
                           {
                             Console.WriteLine($"Retry {retryCount} due to {exception.Message}");
                           });

      _channel = CreateChannel();
    }

    private IModel CreateChannel()
    {
      var connection = _factory.CreateConnection();
      var channel = connection.CreateModel();
      channel.ExchangeDeclare("user_events", ExchangeType.Topic, true);
      return channel;
    }

    public async Task PublishEventAsync<T>(string exchange, string routingKey, T message)
    {
      await _retryPolicy.ExecuteAsync(async () =>
      {
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        _channel.BasicPublish(exchange, routingKey, null, body);
        Console.WriteLine($"Message published to {exchange} with routing key {routingKey}");
      });
    }

    public void Dispose()
    {
      _channel?.Close();
      _channel?.Dispose();
    }

    public void SubscribeToQueue(string queueName, Func<string, Task> onMessageReceived)
    {
      var consumer = new EventingBasicConsumer(_channel);
      consumer.Received += async (model, ea) =>
      {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);

        await onMessageReceived(message);

        _channel.BasicAck(ea.DeliveryTag, false);
      };

      _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
    }
  }
}
