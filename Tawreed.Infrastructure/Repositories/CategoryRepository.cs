using Microsoft.EntityFrameworkCore;
using Tawreed.Domain.Entities;
using Tawreed.Domain.Interfaces;
using Tawreed.Infrastructure.Data;

namespace Tawreed.Infrastructure.Repositories;

public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Category>> GetRootCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(c => c.ParentId == null).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Category>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(c => c.IsActive).ToListAsync(cancellationToken);
    }
}
