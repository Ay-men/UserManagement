namespace HomeManagement.Shared.RabbitMQ;
public interface IRabbitMQService : IDisposable
{
  Task PublishEventAsync<T>(string exchange, string routingKey, T message);
  void SubscribeToQueue(string queueName, Func<string, Task> onMessageReceived);
}