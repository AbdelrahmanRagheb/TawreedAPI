using Tawreed.Domain.Entities;

namespace Tawreed.Domain.Interfaces;

public interface IAuditLogRepository : IGenericRepository<AuditLog>
{
    Task<IReadOnlyList<AuditLog>> GetByActorAsync(Guid actorId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AuditLog>> GetByEntityAsync(string entityType, Guid entityId, CancellationToken cancellationToken = default);
}
