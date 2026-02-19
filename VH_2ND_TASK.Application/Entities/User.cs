namespace VH_2ND_TASK.Application.Entities;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.User;

    public List<RefreshToken> RefreshTokens { get; set; } = new();
}