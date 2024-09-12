using Microsoft.EntityFrameworkCore;
using HomeManagement.UserService.Domain.Entities;

namespace HomeManagement.UserService.Infrastructure.Persistence
{
  public class UserDbContext : DbContext
  {
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

    public DbSet<UserProfile> UserProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<UserProfile>(entity =>
      {
        entity.HasKey(e => e.Id);
        entity.Property(e => e.Email).IsRequired();
        entity.Property(e => e.FirstName).IsRequired();
        entity.Property(e => e.LastName).IsRequired();
      });
    }
  }
}