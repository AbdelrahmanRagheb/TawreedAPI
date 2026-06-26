using Tawreed.Domain.Entities;

namespace Tawreed.Domain.Interfaces;

public interface IDeliveryAssignmentRequestRepository : IGenericRepository<DeliveryAssignmentRequest>
{
    Task<IReadOnlyList<DeliveryAssignmentRequest>> GetPendingByPersonAsync(Guid deliveryPersonId, CancellationToken cancellationToken = default);
    Task<DeliveryAssignmentRequest?> GetByOrderAndPersonAsync(Guid orderId, Guid deliveryPersonId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DeliveryAssignmentRequest>> GetByOrderAsync(Guid orderId, CancellationToken cancellationToken = default);
}
