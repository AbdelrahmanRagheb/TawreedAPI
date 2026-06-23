using Tawreed.Domain.Entities;

namespace Tawreed.Domain.Interfaces;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<IReadOnlyList<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Category>> GetActiveAsync(CancellationToken cancellationToken = default);
}
