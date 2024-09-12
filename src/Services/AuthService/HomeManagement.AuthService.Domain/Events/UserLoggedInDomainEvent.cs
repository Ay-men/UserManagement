using MediatR;

namespace HomeManagement.AuthService.Domain.Events
{
  public class UserLoggedInDomainEvent : INotification
  {
    public Guid UserId { get; }
    public DateTime LoggedInAt { get; }

    public UserLoggedInDomainEvent(Guid userId, DateTime loggedInAt)
    {
      UserId = userId;
      LoggedInAt = loggedInAt;
    }
  }
}