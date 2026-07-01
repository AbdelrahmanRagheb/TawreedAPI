using Microsoft.EntityFrameworkCore;
using Tawreed.Domain.Entities;
using Tawreed.Domain.Interfaces;
using Tawreed.Infrastructure.Data;

namespace Tawreed.Infrastructure.Repositories;

public class GroupOrderRepository : GenericRepository<GroupOrder>, IGroupOrderRepository
{
    public GroupOrderRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<IReadOnlyList<GroupOrder>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(o => o.Items).ThenInclude(i => i.SupplierProduct)
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .Include(o => o.Participants)
            .Include(o => o.Region)
            .Include(o => o.Creator).ThenInclude(c => c.User)
            .Include(o => o.Deliveries)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<GroupOrder>> GetByCreatorAsync(Guid creatorId, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(o => o.CreatorId == creatorId).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<GroupOrder>> GetBySupplierAsync(Guid supplierId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(o => o.Creator).ThenInclude(c => c.User)
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .Include(o => o.Items).ThenInclude(i => i.ParticipantItems)
            .Include(o => o.Participants)
            .Include(o => o.Events)
            .Include(o => o.Region)
            .Where(o => o.Items != null && o.Items.Any(i => i.SupplierId == supplierId))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<GroupOrder>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(o => o.Status == status).ToListAsync(cancellationToken);
    }

    public async Task<GroupOrder?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(o => o.Creator).ThenInclude(c => c.User)
            .Include(o => o.Items).ThenInclude(i => i.Product).ThenInclude(p => p.Unit)
            .Include(o => o.Items).ThenInclude(i => i.SupplierProduct).ThenInclude(sp => sp.PricingTiers)
            .Include(o => o.Items).ThenInclude(i => i.Supplier)
            .Include(o => o.Items).ThenInclude(i => i.ParticipantItems)
            .Include(o => o.Participants).ThenInclude(p => p.Buyer)
            .Include(o => o.Participants).ThenInclude(p => p.Items).ThenInclude(pi => pi.GroupOrderItem).ThenInclude(goi => goi.Product)
            .Include(o => o.Events).ThenInclude(e => e.Creator)
            .Include(o => o.Region)
            .Include(o => o.Deliveries).ThenInclude(d => d.DeliveryPerson)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<GroupOrder>> GetAllWithAdminDetailsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(o => o.Creator).ThenInclude(c => c.User)
            .Include(o => o.Region)
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .Include(o => o.Participants)
            .ToListAsync(cancellationToken);
    }
}
