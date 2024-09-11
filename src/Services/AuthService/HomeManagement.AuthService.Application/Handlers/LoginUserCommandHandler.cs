using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using HomeManagement.AuthService.Application.DTOs;
using HomeManagement.AuthService.Domain.Entities;
using HomeManagement.AuthService.Domain.Services;
using HomeManagement.AuthService.Domain.Events;

namespace HomeManagement.AuthService.Application.Commands
{
  public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, LoginResponseDto>
  {
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IMapper _mapper;
    private readonly TokenService _tokenService;

    public LoginUserCommandHandler(UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper, TokenService tokenService)
    {
      _userManager = userManager;
      _signInManager = signInManager;
      _mapper = mapper;
      _tokenService = tokenService;
    }

    public async Task<LoginResponseDto> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
      var user = await _userManager.FindByEmailAsync(request.LoginDto.Email);
      if (user == null)
      {
        throw new ApplicationException("User not found.");
      }

      var result = await _signInManager.CheckPasswordSignInAsync(user, request.LoginDto.Password, false);
      if (!result.Succeeded)
      {
        throw new ApplicationException("Invalid login attempt.");
      }

      var roles = await _userManager.GetRolesAsync(user);
      var accessToken = _tokenService.GenerateAccessToken(user, roles);
      var refreshToken = _tokenService.GenerateRefreshToken(request.LoginDto.IpAddress, user.Id);

      user.AddRefreshToken(refreshToken);
      user.UpdateLastLogin();
      await _userManager.UpdateAsync(user);

      // Raise domain event
      await Domain.DomainEvents.DomainEvents.Raise(new UserLoggedInDomainEvent(user.Id, DateTime.UtcNow));

      return new LoginResponseDto
      {
        AccessToken = accessToken,
        RefreshToken = refreshToken.Token,
        User = _mapper.Map<UserDto>(user)
      };
    }
  }
}