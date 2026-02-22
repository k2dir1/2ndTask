using Microsoft.EntityFrameworkCore;
using VH_2ND_TASK.Application.Abstractions.Persistence;
using VH_2ND_TASK.Application.Entities;
using VH_2ND_TASK.Infrastructure.Data;

namespace VH_2ND_TASK.Infrastructure.Persistence;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    public UserRepository(AppDbContext db)
    {
        _db = db;
        Console.WriteLine($"Repo DbContext: {_db.GetHashCode()}");
    }
    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct)
    {
        var exists = await _db.Users
            .AnyAsync(x => x.Email == email, ct);

        return exists;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(x => x.Email == email, ct);

        return user;
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken ct)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        return user;
    }

    public async Task AddAsync(User user, CancellationToken ct)
    {
        await _db.Users.AddAsync(user, ct);
    }
}