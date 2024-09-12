namespace HomeManagement.Shared.Exceptions
{
  public class ServiceUnavailableException : Exception
  {
    public ServiceUnavailableException(string serviceName)
        : base($"{serviceName} is currently unavailable.")
    { }
  }
}
