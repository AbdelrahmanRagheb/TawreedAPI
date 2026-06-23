using Tawreed.Application.Common.Models;

namespace Tawreed.Application.Interfaces;

public interface IPricingTierService
{
    Task<IReadOnlyList<PricingTierDto>> GetByProductAsync(Guid supplierProductId, CancellationToken cancellationToken = default);
    Task<PricingTierDto> CreateAsync(Guid supplierProductId, CreatePricingTierRequest request, CancellationToken cancellationToken = default);
    Task<PricingTierDto> UpdateAsync(Guid tierId, UpdatePricingTierRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid tierId, CancellationToken cancellationToken = default);
}
