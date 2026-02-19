using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VH_2ND_TASK.Application.Abstractions.Auth;
using VH_2ND_TASK.Application.DTOs;

namespace VH_2ND_TASK.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;

    public AuthController(IAuthService auth)
    {
        _auth = auth;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest req, CancellationToken ct)
        => Ok(await _auth.RegisterAsync(req, ct));

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest req, CancellationToken ct)
        => Ok(await _auth.LoginAsync(req, ct));

    [Authorize(Roles = "CEO")]
    [HttpPost("set-role")]
    public async Task<IActionResult> SetRole([FromBody] SetUserRoleRequest req, CancellationToken ct)
        => Ok(await _auth.SetRoleAsync(req, ct));

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest req, CancellationToken ct)
        => Ok(await _auth.RefreshAsync(req, ct));
}