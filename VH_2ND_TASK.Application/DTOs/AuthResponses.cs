namespace VH_2ND_TASK.Application.DTOs;

public record RegisterResponse(int Id, string Email);

public record LoginResponse(string AccessToken, string RefreshToken, int UserId);

public record SetRoleResponse(int Id, string Email, string Role);

public record RefreshResponse(string AccessToken, string RefreshToken);