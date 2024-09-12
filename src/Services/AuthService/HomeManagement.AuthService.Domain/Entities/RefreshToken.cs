namespace HomeManagement.AuthService.Domain.Entities
{
  public class RefreshToken
  {
    public Guid Id { get; private set; }
    public string Token { get; private set; }
    public DateTime Expires { get; private set; }
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public DateTime Created { get; private set; }
    public string CreatedByIp { get; private set; }
    public DateTime? Revoked { get; private set; }
    public string RevokedByIp { get; private set; }
    public string ReplacedByToken { get; private set; }
    public bool IsActive => Revoked == null && !IsExpired;
    public virtual Guid UserId { get; set; }
    public virtual User User { get; set; }



    private RefreshToken() { }

    public RefreshToken(string token, DateTime expires, string createdByIp, Guid userId)
    {
      Id = Guid.NewGuid();
      Token = token;
      Expires = expires;
      Created = DateTime.UtcNow;
      CreatedByIp = createdByIp;
      UserId = userId;
    }

    public void Revoke(string revokedByIp, string replacedByToken = null)
    {
      Revoked = DateTime.UtcNow;
      RevokedByIp = revokedByIp;
      ReplacedByToken = replacedByToken;
    }
  }
}