using Microsoft.AspNetCore.Mvc;
using MediatR;
using HomeManagement.UserService.Application.Commands;
using HomeManagement.UserService.Application.Queries;
using HomeManagement.UserService.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System.Text;
using RabbitMQ.Client;

namespace HomeManagement.UserService.Api.Controllers
{
  [Authorize(Roles = "User")]
  [Authorize(AuthenticationSchemes = "Bearer")]
  [ApiController]
  [Route("api/users-profiles")]
  public class UserProfilesController : ControllerBase
  {
    private readonly IMediator _mediator;

    public UserProfilesController(IMediator mediator)
    {
      _mediator = mediator;
    }
    [HttpPost("test-rabbitmq")]
    public IActionResult TestRabbitMQ()
    {
      var factory = new ConnectionFactory()
      {
        HostName = "localhost",
        UserName = "guest",
        Password = "guest",
        Port = 5672
      };

      using var connection = factory.CreateConnection();
      using var channel = connection.CreateModel();

      var testEvent = new { Message = "Test message" };
      var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(testEvent));

      channel.BasicPublish(exchange: "user_events",
                           routingKey: "user.created",
                           basicProperties: null,
                           body: body);

      // _logger.LogInformation("Test message sent to RabbitMQ");
      return Ok("Test message sent");
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUserProfile(Guid id, UpdateUserProfileCommand command)
    {
      return Ok();
      if (id != command.UserId)
      {
        return BadRequest();
      }

      await _mediator.Send(command);
      return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserProfile>> GetUserProfile(Guid id)
    {
      return Ok();

      var query = new GetUserProfileQuery { UserId = id };
      var result = await _mediator.Send(query);
      if (result == null)
      {
        return NotFound();
      }
      return Ok(result);
    }
  }
}