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
            .Include(d => d.Invoice)
            .Where(d => d.DeliveryPersonId == deliveryPersonId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Delivery>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(d => d.Status == status).ToListAsync(cancellationToken);
    }

    public async Task<Delivery?> GetByInvoiceAsync(Guid invoiceId, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(d => d.InvoiceId == invoiceId, cancellationToken);
    }
}
