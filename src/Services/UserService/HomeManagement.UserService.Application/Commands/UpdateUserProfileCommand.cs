using HomeManagement.UserService.Domain.Entities;
using HomeManagement.UserService.Domain.Interfaces;
using MediatR;

namespace HomeManagement.UserService.Application.Commands
{
  public class UpdateUserProfileCommand : IRequest<Unit>
  {
    public Guid UserId { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Address { get; set; }
  }

  public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, Unit>
  {
    private readonly IUserProfileRepository _userProfileRepository;

    public UpdateUserProfileCommandHandler(IUserProfileRepository userProfileRepository)
    {
      _userProfileRepository = userProfileRepository;
    }

    public async Task<Unit> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
      var userProfile = await _userProfileRepository.GetByIdAsync(request.UserId);
      if (userProfile == null)
      {
        throw new NotFoundException(nameof(UserProfile), request.UserId);
      }

      userProfile.UpdateProfile(request.PhoneNumber, request.DateOfBirth, request.Address);
      await _userProfileRepository.UpdateAsync(userProfile);
      return Unit.Value;
    }
  }
}



public class NotFoundException : Exception
{
  public NotFoundException(string name, object key)
      : base($"Entity \"{name}\" ({key}) was not found.")
  {
  }
}