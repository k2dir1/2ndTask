using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VH_2ND_TASK.Data;
using VH_2ND_TASK.DTOs;
using VH_2ND_TASK.Models;
using VH_2ND_TASK.Services;


namespace VH_2ND_TASK.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IJwtService _jwt;
    private readonly PasswordHasher<User> _hasher = new();

    public AuthController(AppDbContext db, IJwtService jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest req, CancellationToken cancellationToken)
    {
        var exists = await _db.Users.AnyAsync(x => x.Email == req.Email, cancellationToken);
        if (exists) return BadRequest("email kullaniliyor");

        var user = new User { Email = req.Email };
        user.PasswordHash = _hasher.HashPassword(user, req.Password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);

        return Ok(new { user.Id, user.Email });
    }

   
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest req, CancellationToken cancellationToken)
    {
        var user = await _db.Users.FirstOrDefaultAsync(x => x.Email == req.Email, cancellationToken);
        if (user is null) return Unauthorized("hata mail");

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, req.Password);
        if (result == PasswordVerificationResult.Failed) return Unauthorized("hata password");

        var accessToken = _jwt.CreateToken(user);

        var refreshToken = _jwt.GenerateRefreshToken();
        var refreshTokenHash = _jwt.HashToken(refreshToken);

        var existing = await _db.RefreshTokens
            .FirstOrDefaultAsync(x => x.UserId == user.Id, cancellationToken);

        if (existing is null)
        {
            _db.RefreshTokens.Add(new RefreshToken
            {
                UserId = user.Id,
                TokenHash = refreshTokenHash,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            });
        }
        else
        {
            existing.TokenHash = refreshTokenHash;
            existing.CreatedAt = DateTime.UtcNow;
            existing.ExpiresAt = DateTime.UtcNow.AddDays(7);
        }

        await _db.SaveChangesAsync(cancellationToken);

        return Ok(new { accessToken, refreshToken, userId = user.Id });
    }


    [Authorize(Roles = "CEO")]
    [HttpPost("set-role")]
    public async Task<IActionResult> SetRole([FromBody] SetUserRoleRequest req, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<UserRole>(req.Role, ignoreCase: true, out var newRole))
            return BadRequest("Role must be one of: User, Admin, CEO");

        var user = await _db.Users.FirstOrDefaultAsync(x => x.Id == req.UserId, cancellationToken);
        if (user is null) return NotFound("User not found");

        user.Role = newRole;
        await _db.SaveChangesAsync(cancellationToken);

        return Ok(new { user.Id, user.Email, Role = user.Role.ToString() });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest req, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(req.RefreshToken))
            return BadRequest("refresh token girilmeli");

        var hash = _jwt.HashToken(req.RefreshToken);

        var stored = await _db.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.TokenHash == hash, cancellationToken);

        if (stored is null) return Unauthorized("hatali refresh token");

        if (stored.ExpiresAt <= DateTime.UtcNow)
        {
            
            _db.RefreshTokens.Remove(stored);
            await _db.SaveChangesAsync(cancellationToken);
            return Unauthorized("refresh tokenin suresi bitmistir");
        }

        var newAccessToken = _jwt.CreateToken(stored.User);
        var newRefreshToken = _jwt.GenerateRefreshToken();
        stored.TokenHash = _jwt.HashToken(newRefreshToken);
        stored.CreatedAt = DateTime.UtcNow;
        stored.ExpiresAt = DateTime.UtcNow.AddDays(7);

        await _db.SaveChangesAsync(cancellationToken);

        return Ok(new { accessToken = newAccessToken, refreshToken = newRefreshToken });
    }
}