using System.Security.Cryptography;
using HomeManagement.AuthService.Domain.Exceptions;

namespace HomeManagement.AuthService.Domain.ValueObjects
{
  public class Password
  {
    public string HashedValue { get; }

    private Password(string hashedValue)
    {
      HashedValue = hashedValue;
    }

    public static Password Create(string password)
    {
      if (string.IsNullOrWhiteSpace(password))
        throw new AuthDomainException("Password cannot be empty");

      if (password.Length < 8)
        throw new AuthDomainException("Password must be at least 8 characters long");

      string hashedPassword = HashPassword(password);
      return new Password(hashedPassword);
    }

    private static string HashPassword(string password)
    {
      byte[] salt;
      new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

      var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
      byte[] hash = pbkdf2.GetBytes(20);

      byte[] hashBytes = new byte[36];
      Array.Copy(salt, 0, hashBytes, 0, 16);
      Array.Copy(hash, 0, hashBytes, 16, 20);

      return Convert.ToBase64String(hashBytes);
    }

    public bool Verify(string password)
    {
      byte[] hashBytes = Convert.FromBase64String(HashedValue);

      byte[] salt = new byte[16];
      Array.Copy(hashBytes, 0, salt, 0, 16);

      var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
      byte[] hash = pbkdf2.GetBytes(20);

      for (int i = 0; i < 20; i++)
      {
        if (hashBytes[i + 16] != hash[i])
          return false;
      }
      return true;
    }
  }
}