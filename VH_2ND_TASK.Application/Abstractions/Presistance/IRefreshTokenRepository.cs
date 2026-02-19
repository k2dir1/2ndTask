using VH_2ND_TASK.Application.Entities;

namespace VH_2ND_TASK.Application.Abstractions.Persistence;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByUserIdAsync(int userId, CancellationToken ct);
    Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken ct);

    Task AddAsync(RefreshToken token, CancellationToken ct);
    void Remove(RefreshToken token);
}
