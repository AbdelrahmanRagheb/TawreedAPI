using Tawreed.Domain.Entities;

namespace Tawreed.Domain.Interfaces;

public interface IGroupOrderEventRepository : IGenericRepository<GroupOrderEvent>
{
    Task<IReadOnlyList<GroupOrderEvent>> GetByGroupOrderAsync(Guid groupOrderId, CancellationToken cancellationToken = default);
}
