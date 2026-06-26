using Microsoft.EntityFrameworkCore;
using Tawreed.Domain.Entities;
using Tawreed.Domain.Interfaces;
using Tawreed.Infrastructure.Data;

namespace Tawreed.Infrastructure.Repositories;

public class DeliveryAssignmentRequestRepository : GenericRepository<DeliveryAssignmentRequest>, IDeliveryAssignmentRequestRepository
{
    public DeliveryAssignmentRequestRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<DeliveryAssignmentRequest>> GetPendingByPersonAsync(Guid deliveryPersonId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(r => r.DeliveryPersonId == deliveryPersonId && r.Status == "Pending")
            .Include(r => r.Order).ThenInclude(o => o.Items)
            .Include(r => r.Order).ThenInclude(o => o.Creator).ThenInclude(c => c.User)
            .ToListAsync(cancellationToken);
    }

    public async Task<DeliveryAssignmentRequest?> GetByOrderAndPersonAsync(Guid orderId, Guid deliveryPersonId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(r => r.OrderId == orderId && r.DeliveryPersonId == deliveryPersonId, cancellationToken);
    }

    public async Task<IReadOnlyList<DeliveryAssignmentRequest>> GetByOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(r => r.OrderId == orderId)
            .ToListAsync(cancellationToken);
    }
}
