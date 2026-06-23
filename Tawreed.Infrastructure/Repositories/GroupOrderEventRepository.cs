using Microsoft.EntityFrameworkCore;
using Tawreed.Domain.Entities;
using Tawreed.Domain.Interfaces;
using Tawreed.Infrastructure.Data;

namespace Tawreed.Infrastructure.Repositories;

public class GroupOrderEventRepository : GenericRepository<GroupOrderEvent>, IGroupOrderEventRepository
{
    public GroupOrderEventRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<GroupOrderEvent>> GetByGroupOrderAsync(Guid groupOrderId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(e => e.GroupOrderId == groupOrderId)
            .OrderBy(e => e.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
