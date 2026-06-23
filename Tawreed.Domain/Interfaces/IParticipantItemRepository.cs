using Tawreed.Domain.Entities;

namespace Tawreed.Domain.Interfaces;

public interface IParticipantItemRepository : IGenericRepository<ParticipantItem>
{
    Task<IReadOnlyList<ParticipantItem>> GetByParticipantAsync(Guid participantId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ParticipantItem>> GetByGroupOrderItemAsync(Guid groupOrderItemId, CancellationToken cancellationToken = default);
}
