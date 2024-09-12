using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using HomeManagement.AuthService.Application.Commands;
using HomeManagement.AuthService.Application.Queries;
using HomeManagement.AuthService.Application.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace HomeManagement.AuthService.Api.Controllers
{
  [ApiController]
  [Route("api/auth")]
  public class AuthController : ControllerBase
  {
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
      _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
      var command = new RegisterUserCommand { RegisterDto = registerDto };
      var result = await _mediator.Send(command);
      return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginDto loginDto)
    {
      loginDto.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
      var command = new LoginUserCommand { LoginDto = loginDto };

      var result = await _mediator.Send(command);
      return Ok(result);
    }

    [Authorize(Roles = "User")]
    [HttpGet("user/{id}")]
    public async Task<ActionResult<UserDto>> GetUserById(Guid id)
    {
      var query = new GetUserByIdQuery { UserId = id };
      var result = await _mediator.Send(query);
      if (result == null)
      {
        return NotFound();
      }
      return Ok(result);
    }
  }
}