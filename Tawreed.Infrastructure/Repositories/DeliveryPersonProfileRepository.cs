using Microsoft.EntityFrameworkCore;
using Tawreed.Domain.Entities;
using Tawreed.Domain.Interfaces;
using Tawreed.Infrastructure.Data;

namespace Tawreed.Infrastructure.Repositories;

public class DeliveryPersonProfileRepository : GenericRepository<DeliveryPersonProfile>, IDeliveryPersonProfileRepository
{
    public DeliveryPersonProfileRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<DeliveryPersonProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(p => p.User)
            .Include(p => p.CoverageRegion)
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<DeliveryPersonProfile>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(p => p.User)
            .Include(p => p.CoverageRegion)
            .Where(p => p.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<DeliveryPersonProfile>> GetForRegionAsync(Guid regionId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(p => p.User)
            .Include(p => p.CoverageRegion)
            .Where(p => p.IsActive && p.CoverageRegionId == regionId)
            .ToListAsync(cancellationToken);
    }
}