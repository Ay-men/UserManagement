using MediatR;
using HomeManagement.UserService.Application.Commands;
using HomeManagement.Shared;

namespace HomeManagement.UserService.Application.EventHandlers
{
  public class UserCreatedEventHandler : INotificationHandler<UserCreatedEvent>
  {
    private readonly IMediator _mediator;

    public UserCreatedEventHandler(IMediator mediator)
    {
      _mediator = mediator;
    }

    public async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
      var command = new CreateUserProfileCommand
      {
        UserId = notification.UserId,
        Email = notification.Email,
        FirstName = notification.FirstName,
        LastName = notification.LastName
      };

      await _mediator.Send(command, cancellationToken);
    }
  }
}