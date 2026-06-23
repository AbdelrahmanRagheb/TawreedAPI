using Tawreed.Application.Common.Models;

namespace Tawreed.Application.Interfaces;

public interface ISupplierProductService
{
    Task<IReadOnlyList<SupplierProductDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SupplierProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SupplierProductDto>> GetBySupplierAsync(Guid supplierId, CancellationToken cancellationToken = default);
    Task<SupplierProductDto> AddProductAsync(Guid supplierId, Guid productId, decimal price, int stock, IReadOnlyList<CreatePricingTierRequest>? tiers = null, CancellationToken cancellationToken = default);
    Task<SupplierProductDto> UpdateProductAsync(Guid id, decimal? price, int? stock, bool? isActive, CancellationToken cancellationToken = default);
    Task DeleteProductAsync(Guid id, CancellationToken cancellationToken = default);
}
