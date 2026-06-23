using Microsoft.EntityFrameworkCore;
using Tawreed.Domain.Entities;
using Tawreed.Domain.Interfaces;
using Tawreed.Infrastructure.Data;

namespace Tawreed.Infrastructure.Repositories;

public class ProductImageRepository : GenericRepository<ProductImage>, IProductImageRepository
{
    public ProductImageRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<ProductImage>> GetBySupplierProductAsync(Guid supplierProductId, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(i => i.SupplierProductId == supplierProductId).OrderBy(i => i.SortOrder).ToListAsync(cancellationToken);
    }

    public async Task<ProductImage?> GetCoverImageAsync(Guid supplierProductId, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(i => i.SupplierProductId == supplierProductId && i.IsCover, cancellationToken);
    }
}
