using Microsoft.EntityFrameworkCore;
using Tawreed.Domain.Entities;
using Tawreed.Domain.Interfaces;
using Tawreed.Infrastructure.Data;

namespace Tawreed.Infrastructure.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Product?> GetWithCategoryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(p => p.Category)
            .Include(p => p.Unit)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetAllWithCategoryAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(p => p.Category)
            .Include(p => p.Unit)
            .ToListAsync(cancellationToken);
    }
}
