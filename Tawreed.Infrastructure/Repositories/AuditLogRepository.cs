using Microsoft.EntityFrameworkCore;
using Tawreed.Domain.Entities;
using Tawreed.Domain.Interfaces;
using Tawreed.Infrastructure.Data;

namespace Tawreed.Infrastructure.Repositories;

public class AuditLogRepository : GenericRepository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<AuditLog>> GetByActorAsync(Guid actorId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(l => l.ActorId == actorId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AuditLog>> GetByEntityAsync(string entityType, Guid entityId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(l => l.EntityType == entityType && l.EntityId == entityId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
