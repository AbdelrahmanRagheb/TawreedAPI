using Tawreed.Domain.Entities;

namespace Tawreed.Application.Interfaces;

public interface IJwtService
{
    (string token, string refreshToken) GenerateTokens(User user);
    string GenerateRefreshToken();
    Guid? ValidateToken(string token);
}
