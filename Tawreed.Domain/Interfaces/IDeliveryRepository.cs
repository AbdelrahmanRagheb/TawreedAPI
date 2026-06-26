using Tawreed.Domain.Entities;

namespace Tawreed.Domain.Interfaces;

public interface IDeliveryRepository : IGenericRepository<Delivery>
{
    Task<IReadOnlyList<Delivery>> GetByDeliveryPersonAsync(Guid deliveryPersonId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Delivery>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
    Task<Delivery?> GetByIdWithGroupOrderAsync(Guid id, CancellationToken cancellationToken = default);
}
