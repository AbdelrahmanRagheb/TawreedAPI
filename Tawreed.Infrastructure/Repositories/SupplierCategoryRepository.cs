using Microsoft.EntityFrameworkCore;
using Tawreed.Domain.Entities;
using Tawreed.Domain.Interfaces;
using Tawreed.Infrastructure.Data;

namespace Tawreed.Infrastructure.Repositories;

public class SupplierCategoryRepository : GenericRepository<SupplierCategory>, ISupplierCategoryRepository
{
    public SupplierCategoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<SupplierCategory>> GetBySupplierAsync(Guid supplierId, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(sc => sc.SupplierId == supplierId).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SupplierCategory>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(sc => sc.CategoryId == categoryId).ToListAsync(cancellationToken);
    }
}
