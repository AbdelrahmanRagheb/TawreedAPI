using Tawreed.Application.Common.Models;
using Tawreed.Application.Interfaces;
using Tawreed.Domain.Entities;
using Tawreed.Domain.Interfaces;

namespace Tawreed.Application.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public NotificationService(INotificationRepository notificationRepository, IUnitOfWork unitOfWork)
    {
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<NotificationDto>> GetUnreadAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var notifications = await _notificationRepository.GetUnreadByUserAsync(userId, cancellationToken);
        return notifications.Select(n => new NotificationDto(n.Id, n.UserId, n.Type, n.TitleAr, n.TitleEn,
            n.BodyAr, n.BodyEn, n.Channel, n.IsRead, n.ReadAt, n.RelatedOrderId, n.CreatedAt)).ToList();
    }

    public async Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _notificationRepository.GetUnreadCountAsync(userId, cancellationToken);
    }

    public async Task MarkAsReadAsync(Guid notificationId, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetByIdAsync(notificationId, cancellationToken);
        if (notification is not null)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTimeOffset.UtcNow;
            _notificationRepository.Update(notification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var unread = await _notificationRepository.GetUnreadByUserAsync(userId, cancellationToken);
        foreach (var notification in unread)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTimeOffset.UtcNow;
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
