using MediatR;
using HomeManagement.AuthService.Application.DTOs;

namespace HomeManagement.AuthService.Application.Commands
{
  public class LoginUserCommand : IRequest<LoginResponseDto>
  {
    public LoginDto LoginDto { get; set; }
  }
}