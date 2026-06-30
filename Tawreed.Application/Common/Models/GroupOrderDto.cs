namespace Tawreed.Application.Common.Models;

public record GroupOrderDto(
    Guid Id,
    Guid CreatorId,
    Guid? SupplierId,
    Guid RegionId,
    string Title,
    string? Description,
    string OrderNumber,
    DateTimeOffset DeadlineAt,
    string Status,
    DateTimeOffset? ClosedAt,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
