namespace HomeManagement.AuthService.Application.Exceptions
{
  public class InvalidCommandException : Exception
  {
    public InvalidCommandException(string message) : base(message) { }
  }
}
