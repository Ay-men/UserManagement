using HomeManagement.AuthService.Application.DTOs;
using HomeManagement.AuthService.Domain.Entities;
namespace HomeManagement.UserService.Application.Interfaces
{
  public interface ITokenService
  {
    string GenerateAccessToken(User user, IList<string> roles);
    RefreshToken GenerateRefreshToken(string ipAddress, Guid userId);
  }
}
