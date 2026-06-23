using Microsoft.EntityFrameworkCore;
using Tawreed.Domain.Entities;
using Tawreed.Domain.Interfaces;
using Tawreed.Infrastructure.Data;

namespace Tawreed.Infrastructure.Repositories;

public class GroupOrderItemRepository : GenericRepository<GroupOrderItem>, IGroupOrderItemRepository
{
    public GroupOrderItemRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<GroupOrderItem>> GetByGroupOrderAsync(Guid groupOrderId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(i => i.Product)
            .Include(i => i.SupplierProduct)
            .Where(i => i.GroupOrderId == groupOrderId)
            .ToListAsync(cancellationToken);
    }
}
