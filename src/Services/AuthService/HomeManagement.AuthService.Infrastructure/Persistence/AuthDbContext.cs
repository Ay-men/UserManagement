using HomeManagement.AuthService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace HomeManagement.AuthService.Infrastructure.Persistence
{
  public class AuthDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
  {
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options)
    {
    }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
      base.OnModelCreating(builder);
      builder.Entity<RefreshToken>(entity =>
       {
         entity.HasKey(e => e.Id);
         entity.Property(e => e.Token).IsRequired();
         entity.HasOne(d => d.User)
              .WithMany(p => p.RefreshTokens)
              .HasForeignKey(d => d.UserId)
              .OnDelete(DeleteBehavior.Cascade);
       });
      builder.Entity<User>()
          .HasMany(u => u.RefreshTokens)
          .WithOne(r => r.User)
          .HasForeignKey(r => r.UserId)
        .OnDelete(DeleteBehavior.Restrict);
    }
  }
}