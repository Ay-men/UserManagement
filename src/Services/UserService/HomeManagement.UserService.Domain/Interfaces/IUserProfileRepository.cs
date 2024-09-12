using HomeManagement.UserService.Domain.Entities;

namespace HomeManagement.UserService.Domain.Interfaces
{
  public interface IUserProfileRepository
  {
    Task<UserProfile> GetByIdAsync(Guid id);
    Task<UserProfile> GetByEmailAsync(string email);
    Task AddAsync(UserProfile userProfile);
    Task UpdateAsync(UserProfile userProfile);
  }
}