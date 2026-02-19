using VH_2ND_TASK.Application.Entities;

namespace VH_2ND_TASK.Application.Abstractions.Security;

public interface IJwtService
{
    string CreateToken(User user);
    string GenerateRefreshToken();
    string HashToken(string token);
}