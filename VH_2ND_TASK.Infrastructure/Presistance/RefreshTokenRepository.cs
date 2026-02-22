using Microsoft.EntityFrameworkCore;
using VH_2ND_TASK.Application.Abstractions.Persistence;
using VH_2ND_TASK.Application.Entities;
using VH_2ND_TASK.Infrastructure.Data;

namespace VH_2ND_TASK.Infrastructure.Persistence;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _db;

    public RefreshTokenRepository(AppDbContext db) => _db = db;

    public Task<RefreshToken?> GetByUserIdAsync(int userId, CancellationToken ct)
        => _db.RefreshTokens.FirstOrDefaultAsync(x => x.UserId == userId, ct);

    public Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken ct)
        => _db.RefreshTokens
            .Include(x => x.User) // so Application can do CreateToken(stored.User)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, ct);

    public Task AddAsync(RefreshToken token, CancellationToken ct)
        => _db.RefreshTokens.AddAsync(token, ct).AsTask();

    public void Remove(RefreshToken token)
        => _db.RefreshTokens.Remove(token);
}