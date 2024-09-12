namespace HomeManagement.Shared.Exceptions
{
  public class MessagePublishingException : Exception
  {

    public MessagePublishingException(string message)
        : base(message) { }
    public MessagePublishingException(string message, Exception innerException)
        : base(message, innerException) { }



  }
}
