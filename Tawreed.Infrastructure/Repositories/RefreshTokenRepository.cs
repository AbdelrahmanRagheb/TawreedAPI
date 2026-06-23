using Microsoft.EntityFrameworkCore;
using Tawreed.Domain.Entities;
using Tawreed.Domain.Interfaces;
using Tawreed.Infrastructure.Data;

namespace Tawreed.Infrastructure.Repositories;

public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(t => t.Token == token, cancellationToken);
    }

    public async Task RevokeByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var activeTokens = await DbSet
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var token in activeTokens)
        {
            token.RevokedAt = DateTimeOffset.UtcNow;
        }
    }
}
