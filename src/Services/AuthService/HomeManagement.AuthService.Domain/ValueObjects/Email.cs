using System.Text.RegularExpressions;
using HomeManagement.AuthService.Domain.Exceptions;

namespace HomeManagement.AuthService.Domain.ValueObjects
{
  public class Email
  {
    public string Value { get; }

    private Email(string value)
    {
      Value = value;
    }

    public static Email Create(string email)
    {
      if (string.IsNullOrWhiteSpace(email))
        throw new AuthDomainException("Email cannot be empty");

      email = email.Trim();

      if (email.Length > 100)
        throw new AuthDomainException("Email is too long");

      if (!IsValidEmail(email))
        throw new AuthDomainException("Invalid email format");

      return new Email(email);
    }

    private static bool IsValidEmail(string email)
    {
      // Simple regex for email validation
      string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
      return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
    }
  }
}