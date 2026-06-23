namespace Tawreed.Application.Common.Models;

using Tawreed.Domain.Enums;

public class RegionTreeNodeDto
{
    public Guid Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public RegionType Type { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? ParentName { get; set; }
    public List<RegionTreeNodeDto> Children { get; set; } = [];
}
