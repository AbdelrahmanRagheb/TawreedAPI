using Tawreed.Application.Common.Models;

namespace Tawreed.Application.Interfaces;

public interface INotificationService
{
    Task<IReadOnlyList<NotificationDto>> GetUnreadAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default);
    Task MarkAsReadAsync(Guid notificationId, CancellationToken cancellationToken = default);
    Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default);
}
