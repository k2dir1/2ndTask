namespace VH_2ND_TASK.DTOs;

public record RegisterRequest(string Email, string Password);
public record LoginRequest(string Email, string Password);

