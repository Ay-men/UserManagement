using Microsoft.EntityFrameworkCore;
using HomeManagement.UserService.Domain.Entities;
using HomeManagement.UserService.Domain.Interfaces;
using HomeManagement.UserService.Infrastructure.Persistence;

namespace HomeManagement.UserService.Infrastructure.Repositories
{
  public class UserProfileRepository : IUserProfileRepository
  {
    private readonly UserDbContext _context;

    public UserProfileRepository(UserDbContext context)
    {
      _context = context;
    }

    public async Task<UserProfile> GetByIdAsync(Guid id)
    {
      return await _context.UserProfiles.FindAsync(id);
    }

    public async Task<UserProfile> GetByEmailAsync(string email)
    {
      return await _context.UserProfiles.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task AddAsync(UserProfile userProfile)
    {
      await _context.UserProfiles.AddAsync(userProfile);
      await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(UserProfile userProfile)
    {
      _context.UserProfiles.Update(userProfile);
      await _context.SaveChangesAsync();
    }
  }
}