using FluentValidation;
using HomeManagement.AuthService.Application.DTOs;

namespace HomeManagement.AuthService.Application.Validators
{
  public class RegisterDtoValidator : AbstractValidator<RegisterDto>
  {
    public RegisterDtoValidator()
    {
      RuleFor(x => x.Email)
          .NotEmpty().WithMessage("Email is required.")
          .EmailAddress().WithMessage("A valid email address is required.");

      RuleFor(x => x.Password)
          .NotEmpty().WithMessage("Password is required.")
          .MinimumLength(6).WithMessage("Password must be at least 6 characters long.")
          .Matches(@"[A-Z]+").WithMessage("Password must contain at least one uppercase letter.")
          .Matches(@"[a-z]+").WithMessage("Password must contain at least one lowercase letter.")
          .Matches(@"[0-9]+").WithMessage("Password must contain at least one number.");

      RuleFor(x => x.FirstName)
          .NotEmpty().WithMessage("First name is required.");

      RuleFor(x => x.LastName)
          .NotEmpty().WithMessage("Last name is required.");
    }
  }
}