using Microsoft.EntityFrameworkCore;
using Tawreed.Domain.Entities;
using Tawreed.Domain.Interfaces;
using Tawreed.Infrastructure.Data;

namespace Tawreed.Infrastructure.Repositories;

public class GroupOrderParticipantRepository : GenericRepository<GroupOrderParticipant>, IGroupOrderParticipantRepository
{
    public GroupOrderParticipantRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<GroupOrderParticipant>> GetByGroupOrderAsync(Guid groupOrderId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(p => p.Buyer)
            .Where(p => p.GroupOrderId == groupOrderId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<GroupOrderParticipant>> GetByBuyerAsync(Guid buyerId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(p => p.GroupOrder)
            .Where(p => p.BuyerId == buyerId)
            .ToListAsync(cancellationToken);
    }
}
