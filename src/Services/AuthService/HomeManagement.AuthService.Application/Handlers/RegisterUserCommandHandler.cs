using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using HomeManagement.AuthService.Application.DTOs;
using HomeManagement.AuthService.Domain.Entities;
using HomeManagement.AuthService.Domain.Interfaces;
using HomeManagement.Shared;
using HomeManagement.Shared.RabbitMQ;

namespace HomeManagement.AuthService.Application.Commands
{
  public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserDto>
  {
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;

    private readonly IRabbitMQService _rabbitMQService;

    public RegisterUserCommandHandler(UserManager<User> userManager, IMapper mapper, IRabbitMQService rabbitMQService)
    {
      _userManager = userManager;
      _mapper = mapper;
      _rabbitMQService = rabbitMQService;
    }

    public async Task<UserDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
      var user = _mapper.Map<User>(request.RegisterDto);

      var result = await _userManager.CreateAsync(user, request.RegisterDto.Password);

      if (!result.Succeeded)
      {
        throw new ApplicationException($"User registration failed: {string.Join(", ", result.Errors)}");
      }

      await _userManager.AddToRoleAsync(user, "User");

      var userCreatedEvent = new UserCreatedEvent(user.Id, user.UserName, user.Email, user.FirstName, user.LastName);
      await _rabbitMQService.PublishEventAsync("user_events", "user.created", userCreatedEvent);

      return _mapper.Map<UserDto>(user);
    }
  }
}