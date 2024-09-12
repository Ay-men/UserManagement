using HomeManagement.AuthService.Domain.Entities;
using HomeManagement.AuthService.Domain.Interfaces;
using HomeManagement.AuthService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HomeManagement.AuthService.Infrastructure.Repositories
{
  public class UserRepository : IUserRepository
  {
    private readonly AuthDbContext _context;

    public UserRepository(AuthDbContext context)
    {
      _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<User> GetByIdAsync(Guid id)
    {
      return await _context.Users.FindAsync(id);
    }

    public async Task<User> GetByEmailAsync(string email)
    {
      return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User> GetByUsernameAsync(string username)
    {
      return await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
    }

    public async Task AddAsync(User user)
    {
      if (user == null) throw new ArgumentNullException(nameof(user));
      await _context.Users.AddAsync(user);
      await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
      if (user == null) throw new ArgumentNullException(nameof(user));
      _context.Users.Update(user);
      await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(User user)
    {
      if (user == null) throw new ArgumentNullException(nameof(user));
      _context.Users.Remove(user);
      await _context.SaveChangesAsync();
    }
  }
}