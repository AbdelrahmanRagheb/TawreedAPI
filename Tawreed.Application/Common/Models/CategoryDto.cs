namespace Tawreed.Application.Common.Models;

public record CategoryDto(
    Guid Id,
    string NameAr,
    string NameEn,
    Guid? ParentId,
    string? IconUrl,
    int SortOrder,
    bool IsActive);
