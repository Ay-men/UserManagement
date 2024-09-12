using MediatR;

namespace HomeManagement.Shared;

public class UserCreatedEvent : INotification
{
  public Guid UserId { get; }
  public string UserName { get; }
  public string Email { get; }
  public string FirstName { get; }
  public string LastName { get; }

  public UserCreatedEvent(Guid userId, string userName, string email, string firstName, string lastName)
  {
    UserId = userId;
    UserName = userName;
    Email = email;
    FirstName = firstName;
    LastName = lastName;
  }
}