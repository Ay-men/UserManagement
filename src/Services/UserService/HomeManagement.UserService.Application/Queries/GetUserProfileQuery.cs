using System;
using MediatR;
using HomeManagement.UserService.Domain.Entities;
using HomeManagement.UserService.Domain.Interfaces;

namespace HomeManagement.UserService.Application.Queries
{
  public class GetUserProfileQuery : IRequest<UserProfile>
  {
    public Guid UserId { get; set; }
  }

  public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfile>
  {
    private readonly IUserProfileRepository _userProfileRepository;

    public GetUserProfileQueryHandler(IUserProfileRepository userProfileRepository)
    {
      _userProfileRepository = userProfileRepository;
    }

    public async Task<UserProfile> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
      return await _userProfileRepository.GetByIdAsync(request.UserId);
    }
  }
}