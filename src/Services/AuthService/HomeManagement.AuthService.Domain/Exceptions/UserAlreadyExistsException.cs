namespace HomeManagement.AuthService.Domain.Exceptions
{
  public class UserAlreadyExistsException : DomainException
  {
    public UserAlreadyExistsException(string email)
        : base($"A user with email '{email}' already exists.")
    { }
  }
}
