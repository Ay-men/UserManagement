using MediatR;

namespace HomeManagement.AuthService.Domain.Events
{
  public class UserCreatedDomainEvent : INotification
  {
    public Guid UserId { get; }
    public string UserName { get; }
    public string Email { get; }

    public UserCreatedDomainEvent(Guid userId, string userName, string email)
    {
      UserId = userId;
      UserName = userName;
      Email = email;
    }
  }
}