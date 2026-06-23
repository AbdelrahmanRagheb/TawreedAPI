using Tawreed.Domain.Entities;

namespace Tawreed.Domain.Interfaces;

public interface IGroupOrderItemRepository : IGenericRepository<GroupOrderItem>
{
    Task<IReadOnlyList<GroupOrderItem>> GetByGroupOrderAsync(Guid groupOrderId, CancellationToken cancellationToken = default);
}
