using System;
using System.Threading.Tasks;
using HomeManagement.AuthService.Domain.Entities;

namespace HomeManagement.AuthService.Domain.Interfaces
{
  public interface IUserRepository
  {
    Task<User> GetByIdAsync(Guid id);
    Task<User> GetByEmailAsync(string email);
    Task<User> GetByUsernameAsync(string username);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
  }
}