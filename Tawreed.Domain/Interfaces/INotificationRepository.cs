using Tawreed.Domain.Entities;

namespace Tawreed.Domain.Interfaces;

public interface INotificationRepository : IGenericRepository<Notification>
{
    Task<IReadOnlyList<Notification>> GetUnreadByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default);
}
