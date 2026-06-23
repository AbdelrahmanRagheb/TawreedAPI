namespace Tawreed.Application.Common.Models;

public record SupplierProductDto(
    Guid Id,
    Guid SupplierId,
    Guid ProductId,
    string ProductName,
    string? Description,
    Guid CategoryId,
    string CategoryName,
    Guid UnitId,
    string Unit,
    string? ImageUrl,
    decimal Price,
    int Stock,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
