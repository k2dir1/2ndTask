using Microsoft.EntityFrameworkCore;
using VH_2ND_TASK.Models;

namespace VH_2ND_TASK.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Book> Books => Set<Book>();
}