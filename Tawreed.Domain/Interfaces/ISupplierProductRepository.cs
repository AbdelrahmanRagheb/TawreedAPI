using Tawreed.Domain.Entities;

namespace Tawreed.Domain.Interfaces;

public interface ISupplierProductRepository : IGenericRepository<SupplierProduct>
{
    Task<IReadOnlyList<SupplierProduct>> GetBySupplierAsync(Guid supplierId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SupplierProduct>> GetByProductAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<SupplierProduct?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<SupplierProduct?> FindCheapestForProductAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SupplierProduct>> GetForProductsWithTiersAsync(IEnumerable<Guid> productIds, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SupplierProduct>> GetBySupplierWithTiersAsync(Guid supplierId, CancellationToken cancellationToken = default);
}
