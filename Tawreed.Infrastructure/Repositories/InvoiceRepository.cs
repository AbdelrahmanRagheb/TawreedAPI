using Microsoft.EntityFrameworkCore;
using Tawreed.Domain.Entities;
using Tawreed.Domain.Interfaces;
using Tawreed.Infrastructure.Data;

namespace Tawreed.Infrastructure.Repositories;

public class InvoiceRepository : GenericRepository<Invoice>, IInvoiceRepository
{
    public InvoiceRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Invoice>> GetByBuyerAsync(Guid buyerId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(i => i.GroupOrder)
            .Include(i => i.Participant).ThenInclude(p => p.Items).ThenInclude(pi => pi.GroupOrderItem).ThenInclude(goi => goi.Product)
            .Include(i => i.GroupOrder).ThenInclude(go => go.Deliveries).ThenInclude(d => d.DeliveryPerson)
            .Where(i => i.BuyerId == buyerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Invoice>> GetByGroupOrderAsync(Guid groupOrderId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(i => i.Buyer).ThenInclude(b => b.User)
            .Include(i => i.Buyer).ThenInclude(b => b.Region)
            .Include(i => i.Participant).ThenInclude(p => p.Items)
            .Where(i => i.GroupOrderId == groupOrderId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Invoice?> GetByIdWithGroupOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(i => i.GroupOrder).ThenInclude(go => go.Deliveries)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }
}
