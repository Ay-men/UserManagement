using System;
using MediatR;
using HomeManagement.AuthService.Application.DTOs;

namespace HomeManagement.AuthService.Application.Queries
{
  public class GetUserByIdQuery : IRequest<UserDto>
  {
    public Guid UserId { get; set; }
  }
}