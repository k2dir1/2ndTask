using Microsoft.AspNetCore.Identity;
using VH_2ND_TASK.Application.Abstractions.Auth;
using VH_2ND_TASK.Application.Abstractions.Persistence;
using VH_2ND_TASK.Application.Abstractions.Security;
using VH_2ND_TASK.Application.DTOs;
using VH_2ND_TASK.Application.Entities;

namespace VH_2ND_TASK.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IUnitOfWork _uow;
    private readonly IJwtService _jwt;

    private readonly PasswordHasher<User> _hasher = new();

    public AuthService(
        IUserRepository users,
        IRefreshTokenRepository refreshTokens,
        IUnitOfWork uow,
        IJwtService jwt) 
    {
        _users = users;
        _refreshTokens = refreshTokens;
        _uow = uow;
        _jwt = jwt;
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest req, CancellationToken ct)
    {
        var exists = await _users.EmailExistsAsync(req.Email, ct);
        if (exists) throw new InvalidOperationException("email kullaniliyor");

        var user = new User { Email = req.Email };
        user.PasswordHash = _hasher.HashPassword(user, req.Password);

        await _users.AddAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        return new RegisterResponse(user.Id, user.Email);
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest req, CancellationToken ct)
    {
        var user = await _users.GetByEmailAsync(req.Email, ct);
        if (user is null) throw new UnauthorizedAccessException("hata mail");

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, req.Password);
        if (result == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException("hata password");

        var accessToken = _jwt.CreateToken(user);

        var refreshToken = _jwt.GenerateRefreshToken();
        var refreshTokenHash = _jwt.HashToken(refreshToken);

        var existing = await _refreshTokens.GetByUserIdAsync(user.Id, ct);

        if (existing is null)
        {
            await _refreshTokens.AddAsync(new RefreshToken
            {
                UserId = user.Id,
                TokenHash = refreshTokenHash,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            }, ct);
        }
        else
        {
            existing.TokenHash = refreshTokenHash;
            existing.CreatedAt = DateTime.UtcNow;
            existing.ExpiresAt = DateTime.UtcNow.AddDays(7);
        }

        await _uow.SaveChangesAsync(ct);

        return new LoginResponse(accessToken, refreshToken, user.Id);
    }

    public async Task<SetRoleResponse> SetRoleAsync(SetUserRoleRequest req, CancellationToken ct)
    {
        if (!Enum.TryParse<UserRole>(req.Role, ignoreCase: true, out var newRole))
            throw new InvalidOperationException("Role must be one of: User, Admin, CEO");

        var user = await _users.GetByIdAsync(req.UserId, ct);
        if (user is null) throw new KeyNotFoundException("User not found");

        user.Role = newRole;
        await _uow.SaveChangesAsync(ct);

        return new SetRoleResponse(user.Id, user.Email, user.Role.ToString());
    }

    public async Task<RefreshResponse> RefreshAsync(RefreshRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.RefreshToken))
            throw new InvalidOperationException("refresh token girilmeli");

        var hash = _jwt.HashToken(req.RefreshToken);

        var stored = await _refreshTokens.GetByHashAsync(hash, ct);
        if (stored is null) throw new UnauthorizedAccessException("hatali refresh token");

        if (stored.ExpiresAt <= DateTime.UtcNow)
        {
            _refreshTokens.Remove(stored);
            await _uow.SaveChangesAsync(ct);
            throw new UnauthorizedAccessException("refresh tokenin suresi bitmistir");
        }

      
        var user = stored.User;
        if (user is null) throw new InvalidOperationException("refresh token user yok");

        var newAccessToken = _jwt.CreateToken(user);
        var newRefreshToken = _jwt.GenerateRefreshToken();

        stored.TokenHash = _jwt.HashToken(newRefreshToken);
        stored.CreatedAt = DateTime.UtcNow;
        stored.ExpiresAt = DateTime.UtcNow.AddDays(7);

        await _uow.SaveChangesAsync(ct);

        return new RefreshResponse(newAccessToken, newRefreshToken);
    }
}