using HomeManagement.UserService.Domain.Entities;
using HomeManagement.UserService.Domain.Interfaces;
using MediatR;

namespace HomeManagement.UserService.Application.Commands
{
  public class CreateUserProfileCommand : IRequest<Guid>
  {
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
  }

  public class CreateUserProfileCommandHandler : IRequestHandler<CreateUserProfileCommand, Guid>
  {
    private readonly IUserProfileRepository _userProfileRepository;

    public CreateUserProfileCommandHandler(IUserProfileRepository userProfileRepository)
    {
      _userProfileRepository = userProfileRepository;
    }

    public async Task<Guid> Handle(CreateUserProfileCommand request, CancellationToken cancellationToken)
    {
      var userProfile = new UserProfile(request.UserId, request.Email, request.FirstName, request.LastName);
      await _userProfileRepository.AddAsync(userProfile);
      return userProfile.Id;
    }
  }
}