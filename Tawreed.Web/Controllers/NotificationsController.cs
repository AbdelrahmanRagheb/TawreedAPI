using Microsoft.AspNetCore.Mvc;
using Tawreed.Application.Common.Models;
using Tawreed.Application.Interfaces;

namespace Tawreed.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet("unread/{userId:guid}")]
    public async Task<ActionResult<IReadOnlyList<NotificationDto>>> GetUnread(Guid userId, CancellationToken cancellationToken)
    {
        var notifications = await _notificationService.GetUnreadAsync(userId, cancellationToken);
        return Ok(notifications);
    }

    [HttpGet("unread-count/{userId:guid}")]
    public async Task<ActionResult<int>> GetUnreadCount(Guid userId, CancellationToken cancellationToken)
    {
        var count = await _notificationService.GetUnreadCountAsync(userId, cancellationToken);
        return Ok(count);
    }

    [HttpPatch("{id:guid}/read")]
    public async Task<ActionResult> MarkAsRead(Guid id, CancellationToken cancellationToken)
    {
        await _notificationService.MarkAsReadAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPatch("read-all/{userId:guid}")]
    public async Task<ActionResult> MarkAllAsRead(Guid userId, CancellationToken cancellationToken)
    {
        await _notificationService.MarkAllAsReadAsync(userId, cancellationToken);
        return NoContent();
    }
}
