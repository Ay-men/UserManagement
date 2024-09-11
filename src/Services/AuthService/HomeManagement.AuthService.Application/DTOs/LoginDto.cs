namespace HomeManagement.AuthService.Application.DTOs
{
  public class LoginDto
  {
    public string Email { get; set; }
    public string Password { get; set; }
    public string IpAddress { get; set; }
  }
}