using Tawreed.Domain.Entities;

namespace Tawreed.Domain.Interfaces;

public interface IDeliveryPersonProfileRepository : IGenericRepository<DeliveryPersonProfile>
{
    Task<DeliveryPersonProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DeliveryPersonProfile>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DeliveryPersonProfile>> GetForRegionAsync(Guid regionId, CancellationToken cancellationToken = default);
}