namespace Tawreed.Application.Common.Models;

public record NotificationDto(
    Guid Id,
    Guid UserId,
    string Type,
    string TitleAr,
    string TitleEn,
    string? BodyAr,
    string? BodyEn,
    string Channel,
    bool IsRead,
    DateTimeOffset? ReadAt,
    Guid? RelatedOrderId,
    DateTime CreatedAt);
