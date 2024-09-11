using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using HomeManagement.AuthService.Application.DTOs;
using HomeManagement.AuthService.Domain.Entities;
using HomeManagement.AuthService.Domain.Interfaces;
using HomeManagement.AuthService.Domain.Events;

namespace HomeManagement.AuthService.Application.Commands
{
  public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserDto>
  {
    private readonly UserManager<User> _userManager;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;

    public RegisterUserCommandHandler(UserManager<User> userManager, IMapper mapper, IUserRepository userRepository)
    {
      _userManager = userManager;
      _mapper = mapper;
      _userRepository = userRepository;
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

      // Raise domain event
      await Domain.DomainEvents.DomainEvents.Raise(new UserCreatedDomainEvent(user.Id, user.UserName, user.Email));

      return _mapper.Map<UserDto>(user);
    }
  }
}