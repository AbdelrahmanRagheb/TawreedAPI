using Microsoft.EntityFrameworkCore;
using Tawreed.Domain.Entities;
using Tawreed.Domain.Interfaces;
using Tawreed.Infrastructure.Data;

namespace Tawreed.Infrastructure.Repositories;

public class ParticipantItemRepository : GenericRepository<ParticipantItem>, IParticipantItemRepository
{
    public ParticipantItemRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<ParticipantItem>> GetByParticipantAsync(Guid participantId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(i => i.GroupOrderItem).ThenInclude(goi => goi.SupplierProduct).ThenInclude(sp => sp.Product)
            .Where(i => i.ParticipantId == participantId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ParticipantItem>> GetByGroupOrderItemAsync(Guid groupOrderItemId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(i => i.Participant).ThenInclude(p => p.Buyer)
            .Where(i => i.GroupOrderItemId == groupOrderItemId)
            .ToListAsync(cancellationToken);
    }
}
