namespace HomeManagement.AuthService.Application.DTOs
{
  public class LoginResponseDto
  {
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public UserDto User { get; set; }
  }
}