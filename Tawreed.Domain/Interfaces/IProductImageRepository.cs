using Tawreed.Domain.Entities;

namespace Tawreed.Domain.Interfaces;

public interface IProductImageRepository : IGenericRepository<ProductImage>
{
    Task<IReadOnlyList<ProductImage>> GetBySupplierProductAsync(Guid supplierProductId, CancellationToken cancellationToken = default);
    Task<ProductImage?> GetCoverImageAsync(Guid supplierProductId, CancellationToken cancellationToken = default);
}
