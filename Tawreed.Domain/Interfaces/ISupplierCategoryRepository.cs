using Tawreed.Domain.Entities;

namespace Tawreed.Domain.Interfaces;

public interface ISupplierCategoryRepository : IGenericRepository<SupplierCategory>
{
    Task<IReadOnlyList<SupplierCategory>> GetBySupplierAsync(Guid supplierId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SupplierCategory>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
}
