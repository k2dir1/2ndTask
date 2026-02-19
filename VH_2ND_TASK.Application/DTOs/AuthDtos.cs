namespace VH_2ND_TASK.Application.DTOs;

public record RegisterRequest(string Email, string Password);
public record LoginRequest(string Email, string Password);

