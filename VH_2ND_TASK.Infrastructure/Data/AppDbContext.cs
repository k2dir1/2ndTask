using Microsoft.EntityFrameworkCore;
using VH_2ND_TASK.Application.Entities;

namespace VH_2ND_TASK.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>()
            .HasMaxLength(20);

     
        modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<RefreshToken>()
             .HasIndex(rt => rt.TokenHash)
             .IsUnique();

        modelBuilder.Entity<RefreshToken>()
            .HasIndex(rt => rt.UserId)
            .IsUnique();
    }
}