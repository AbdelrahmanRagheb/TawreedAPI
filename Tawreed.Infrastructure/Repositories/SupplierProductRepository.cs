using Microsoft.EntityFrameworkCore;
using Tawreed.Domain.Entities;
using Tawreed.Domain.Interfaces;
using Tawreed.Infrastructure.Data;

namespace Tawreed.Infrastructure.Repositories;

public class SupplierProductRepository : GenericRepository<SupplierProduct>, ISupplierProductRepository
{
    public SupplierProductRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<SupplierProduct?> FindCheapestForProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(sp => sp.ProductId == productId && sp.IsActive && sp.Supplier.IsApproved)
            .OrderBy(sp => sp.Price)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SupplierProduct>> GetBySupplierAsync(Guid supplierId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(sp => sp.Product).ThenInclude(p => p.Category)
            .Include(sp => sp.Product).ThenInclude(p => p.Unit)
            .Where(sp => sp.SupplierId == supplierId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SupplierProduct>> GetByProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(sp => sp.ProductId == productId).ToListAsync(cancellationToken);
    }

    public async Task<SupplierProduct?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(sp => sp.Product).ThenInclude(p => p.Category)
            .Include(sp => sp.Product).ThenInclude(p => p.Unit)
            .Include(sp => sp.Images)
            .Include(sp => sp.PricingTiers)
            .FirstOrDefaultAsync(sp => sp.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<SupplierProduct>> GetForProductsWithTiersAsync(IEnumerable<Guid> productIds, CancellationToken cancellationToken = default)
    {
        var productIdsList = productIds.ToList();
        return await DbSet
            .AsNoTracking()
            .Include(sp => sp.Supplier)
            .Include(sp => sp.PricingTiers)
            .Where(sp => productIdsList.Contains(sp.ProductId) && sp.IsActive && sp.Supplier.IsApproved)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SupplierProduct>> GetBySupplierWithTiersAsync(Guid supplierId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(sp => sp.Product).ThenInclude(p => p.Category)
            .Include(sp => sp.Product).ThenInclude(p => p.Unit)
            .Include(sp => sp.PricingTiers)
            .Include(sp => sp.Images)
            .Where(sp => sp.SupplierId == supplierId && sp.IsActive)
            .OrderBy(sp => sp.Product.Name)
            .ToListAsync(cancellationToken);
    }
}
