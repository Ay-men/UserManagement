namespace HomeManagement.Shared.Exceptions
{
  public class RabbitMQConnectionException : Exception
  {
    public RabbitMQConnectionException(string message)
        : base(message)
    {
    }
    public RabbitMQConnectionException(string message, Exception innerException)
        : base(message, innerException) { }
  }

}
