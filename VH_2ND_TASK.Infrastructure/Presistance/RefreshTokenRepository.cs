using Microsoft.EntityFrameworkCore;
using VH_2ND_TASK.Application.Abstractions.Persistence;
using VH_2ND_TASK.Application.Entities;
using VH_2ND_TASK.Infrastructure.Data;

namespace VH_2ND_TASK.Infrastructure.Persistence;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _db;

    public RefreshTokenRepository(AppDbContext db)
    {
        _db = db;
        Console.WriteLine($"RefreshRepo DbContext: {_db.GetHashCode()}");
    }

    public async Task<RefreshToken?> GetByUserIdAsync(int userId, CancellationToken ct)
    {
        var token = await _db.RefreshTokens
            .FirstOrDefaultAsync(x => x.UserId == userId, ct);

        return token;
    }

    public async Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken ct)
    {
        var token = await _db.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, ct);

        return token;
    }

    public async Task AddAsync(RefreshToken token, CancellationToken ct)
    {
        await _db.RefreshTokens.AddAsync(token, ct);
    }

    public void Remove(RefreshToken token)
    {
        _db.RefreshTokens.Remove(token);
    }
}