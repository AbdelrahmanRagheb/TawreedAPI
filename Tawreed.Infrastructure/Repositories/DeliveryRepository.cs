using Microsoft.EntityFrameworkCore;
using Tawreed.Domain.Entities;
using Tawreed.Domain.Interfaces;
using Tawreed.Infrastructure.Data;

namespace Tawreed.Infrastructure.Repositories;

public class DeliveryRepository : GenericRepository<Delivery>, IDeliveryRepository
{
    public DeliveryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Delivery>> GetByDeliveryPersonAsync(Guid deliveryPersonId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(d => d.GroupOrder).ThenInclude(go => go.Invoices)
            .Where(d => d.DeliveryPersonId == deliveryPersonId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Delivery>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(d => d.Status == status).ToListAsync(cancellationToken);
    }

    public async Task<Delivery?> GetByIdWithGroupOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(d => d.GroupOrder).ThenInclude(go => go.Items).ThenInclude(goi => goi.Product)
            .Include(d => d.GroupOrder).ThenInclude(go => go.Invoices)
            .Include(d => d.GroupOrder).ThenInclude(go => go.Participants).ThenInclude(p => p.Items)
            .Include(d => d.GroupOrder).ThenInclude(go => go.Creator).ThenInclude(c => c.User)
            .Include(d => d.GroupOrder).ThenInclude(go => go.Creator).ThenInclude(c => c.Region)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }
}
