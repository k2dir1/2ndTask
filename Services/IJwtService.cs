using VH_2ND_TASK.Models;

namespace VH_2ND_TASK.Services;

public interface IJwtService
{
    string CreateToken(User user);
    string GenerateRefreshToken();
    string HashToken(string token);
}