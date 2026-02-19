using VH_2ND_TASK.Application.Entities;

namespace VH_2ND_TASK.Application.Abstractions.Persistence;

public interface IUserRepository
{
    Task<bool> EmailExistsAsync(string email, CancellationToken ct);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    Task<User?> GetByIdAsync(int id, CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);
}