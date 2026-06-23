using Tawreed.Domain.Entities;

namespace Tawreed.Domain.Interfaces;

public interface IPricingTierRepository : IGenericRepository<PricingTier>
{
    Task<IReadOnlyList<PricingTier>> GetBySupplierProductAsync(Guid supplierProductId, CancellationToken cancellationToken = default);
    Task<PricingTier?> GetTierForQuantityAsync(Guid supplierProductId, int quantity, CancellationToken cancellationToken = default);
}
