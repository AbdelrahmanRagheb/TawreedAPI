namespace Tawreed.Application.Common.Models;

public record ProductDto(
    Guid Id,
    string Name,
    string? Description,
    Guid CategoryId,
    string CategoryName,
    Guid UnitId,
    string Unit,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
