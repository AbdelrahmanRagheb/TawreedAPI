using Microsoft.EntityFrameworkCore;
using Tawreed.Domain.Entities;
using Tawreed.Domain.Interfaces;
using Tawreed.Infrastructure.Data;

namespace Tawreed.Infrastructure.Repositories;

public class PricingTierRepository : GenericRepository<PricingTier>, IPricingTierRepository
{
    public PricingTierRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<PricingTier>> GetBySupplierProductAsync(Guid supplierProductId, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(t => t.SupplierProductId == supplierProductId).OrderBy(t => t.MinQty).ToListAsync(cancellationToken);
    }

    public async Task<PricingTier?> GetTierForQuantityAsync(Guid supplierProductId, int quantity, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(t => t.SupplierProductId == supplierProductId && t.MinQty <= quantity && (t.MaxQty == null || t.MaxQty >= quantity))
            .OrderBy(t => t.MinQty)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
