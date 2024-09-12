using Microsoft.AspNetCore.Identity;

namespace HomeManagement.AuthService.Domain.Entities
{
  public class User : IdentityUser<Guid>
  {
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public virtual List<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();

    private User() { }

    public User(string userName, string email, string firstName, string lastName)
    {
      UserName = userName ?? throw new ArgumentNullException(nameof(userName));
      Email = email ?? throw new ArgumentNullException(nameof(email));
      FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
      LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
      CreatedAt = DateTime.UtcNow;
    }

    public void UpdateLastLogin()
    {
      LastLoginAt = DateTime.UtcNow;
    }

    public void AddRefreshToken(RefreshToken token)
    {
      RefreshTokens.Add(token);
    }


  }
}