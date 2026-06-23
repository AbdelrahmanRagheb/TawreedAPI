using Tawreed.Domain.Entities;

namespace Tawreed.Domain.Interfaces;

public interface IGroupOrderParticipantRepository : IGenericRepository<GroupOrderParticipant>
{
    Task<IReadOnlyList<GroupOrderParticipant>> GetByGroupOrderAsync(Guid groupOrderId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GroupOrderParticipant>> GetByBuyerAsync(Guid buyerId, CancellationToken cancellationToken = default);
}
