using Tawreed.Domain.Entities;

namespace Tawreed.Domain.Interfaces;

public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task RevokeByUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
