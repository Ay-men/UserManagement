using MediatR;
using HomeManagement.AuthService.Application.DTOs;

namespace HomeManagement.AuthService.Application.Commands
{
  public class RegisterUserCommand : IRequest<UserDto>
  {
    public RegisterDto RegisterDto { get; set; }
  }
}