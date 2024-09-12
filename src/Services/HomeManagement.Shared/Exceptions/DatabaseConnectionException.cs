namespace HomeManagement.Shared.Exceptions
{
  public class DatabaseConnectionException : Exception
  {
    public DatabaseConnectionException(string message, Exception innerException)
        : base(message, innerException) { }
  }
}
