using Microsoft.EntityFrameworkCore;
using Tawreed.Domain.Entities;
using Tawreed.Domain.Interfaces;
using Tawreed.Infrastructure.Data;

namespace Tawreed.Infrastructure.Repositories;

public class SupplierApprovalLogRepository : GenericRepository<SupplierApprovalLog>, ISupplierApprovalLogRepository
{
    public SupplierApprovalLogRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<SupplierApprovalLog>> GetBySupplierAsync(Guid supplierId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(l => l.Actor)
            .Where(l => l.SupplierId == supplierId)
            .ToListAsync(cancellationToken);
    }
}
