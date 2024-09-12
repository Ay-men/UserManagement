using HomeManagement.AuthService.Domain.Entities;
using HomeManagement.Shared;
using HomeManagement.Shared.RabbitMQ;

public class UserService
{
  private readonly IRabbitMQService _rabbitMqService;

  public UserService(IRabbitMQService rabbitMqService)
  {
    _rabbitMqService = rabbitMqService;
  }

  public async Task CreateUser(User user)
  {
    // After user creation logic
    var userCreatedEvent = new UserCreatedEvent(user.Id, user.UserName, user.Email, user.FirstName, user.LastName);
    await _rabbitMqService.PublishEventAsync("user_events", "user.created", userCreatedEvent);
  }
}
