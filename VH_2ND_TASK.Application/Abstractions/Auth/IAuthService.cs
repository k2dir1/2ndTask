using VH_2ND_TASK.Application.DTOs;

namespace VH_2ND_TASK.Application.Abstractions.Auth;

public interface IAuthService
{
    Task<RegisterResponse> RegisterAsync(RegisterRequest req, CancellationToken ct);
    Task<LoginResponse> LoginAsync(LoginRequest req, CancellationToken ct);
    Task<SetRoleResponse> SetRoleAsync(SetUserRoleRequest req, CancellationToken ct);
    Task<RefreshResponse> RefreshAsync(RefreshRequest req, CancellationToken ct);
}