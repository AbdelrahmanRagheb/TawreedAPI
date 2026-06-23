using Microsoft.EntityFrameworkCore;
using Tawreed.Domain.Entities;
using Tawreed.Domain.Interfaces;
using Tawreed.Infrastructure.Data;

namespace Tawreed.Infrastructure.Repositories;

public class BuyerRepository : GenericRepository<Buyer>, IBuyerRepository
{
    public BuyerRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Buyer?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(b => b.UserId == userId, cancellationToken);
    }

    public async Task<Buyer?> GetByUserIdWithDetailsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(b => b.User)
            .Include(b => b.Region)
            .FirstOrDefaultAsync(b => b.UserId == userId, cancellationToken);
    }
}
