namespace Tawreed.Application.Common.Models;

using Tawreed.Domain.Enums;

public record RegionDto(
    Guid Id,
    string NameAr,
    string NameEn,
    Guid? ParentId,
    RegionType Type,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
