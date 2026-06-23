using Microsoft.EntityFrameworkCore;
using Tawreed.Domain.Entities;
using Tawreed.Domain.Interfaces;
using Tawreed.Infrastructure.Data;

namespace Tawreed.Infrastructure.Repositories;

public class SupplierRepository : GenericRepository<Supplier>, ISupplierRepository
{
    public SupplierRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Supplier?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);
    }

    public async Task<Supplier?> GetByIdWithDetailsAsync(Guid supplierId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(s => s.User)
            .Include(s => s.Region)
            .Include(s => s.SupplierCategories).ThenInclude(sc => sc.Category)
            .FirstOrDefaultAsync(s => s.Id == supplierId && s.IsApproved, cancellationToken);
    }

    public override async Task<IReadOnlyList<Supplier>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(s => s.User)
            .Include(s => s.Region)
            .Include(s => s.SupplierCategories)
                .ThenInclude(sc => sc.Category)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Supplier>> GetApprovedAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(s => s.IsApproved).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Supplier>> GetPendingApprovalAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(s => !s.IsApproved).ToListAsync(cancellationToken);
    }
}
