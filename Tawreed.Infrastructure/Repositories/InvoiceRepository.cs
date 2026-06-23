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
            .Where(i => i.BuyerId == buyerId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Invoice>> GetByGroupOrderAsync(Guid groupOrderId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(i => i.Buyer)
            .Where(i => i.GroupOrderId == groupOrderId)
            .ToListAsync(cancellationToken);
    }
}
