namespace HomeManagement.UserService.Domain.Entities;

public class UserProfile
{
  public Guid Id { get; private set; }
  public string Email { get; private set; }
  public string FirstName { get; private set; }
  public string LastName { get; private set; }
  public string PhoneNumber { get; private set; }
  public DateTime DateOfBirth { get; private set; }
  public string Address { get; private set; }
  public bool IsEmailConfirmed { get; private set; }
  public DateTime CreatedAt { get; private set; }
  public DateTime? UpdatedAt { get; private set; }

  private UserProfile() { } // For EF Core

  public UserProfile(Guid id, string email, string firstName, string lastName)
  {
    Id = id;
    Email = email;
    FirstName = firstName;
    LastName = lastName;
    CreatedAt = DateTime.UtcNow;
    IsEmailConfirmed = false;
  }

  public void UpdateProfile(string phoneNumber, DateTime dateOfBirth, string address)
  {
    PhoneNumber = phoneNumber;
    DateOfBirth = dateOfBirth;
    Address = address;
    UpdatedAt = DateTime.UtcNow;
  }

  public void ConfirmEmail()
  {
    IsEmailConfirmed = true;
    UpdatedAt = DateTime.UtcNow;
  }
}