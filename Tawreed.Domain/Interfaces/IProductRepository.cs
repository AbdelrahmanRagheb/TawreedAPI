using Tawreed.Domain.Entities;

namespace Tawreed.Domain.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<Product?> GetWithCategoryAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetAllWithCategoryAsync(CancellationToken cancellationToken = default);
}
