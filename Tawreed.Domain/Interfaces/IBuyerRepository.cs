using Tawreed.Domain.Entities;

namespace Tawreed.Domain.Interfaces;

public interface IBuyerRepository : IGenericRepository<Buyer>
{
    Task<Buyer?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Buyer?> GetByUserIdWithDetailsAsync(Guid userId, CancellationToken cancellationToken = default);
}
