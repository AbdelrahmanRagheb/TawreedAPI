using Tawreed.Domain.Common;

namespace Tawreed.Domain.Entities;

public class Notification : BaseAuditableEntity
{
    public Guid UserId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string TitleEn { get; set; } = string.Empty;
    public string? BodyAr { get; set; }
    public string? BodyEn { get; set; }
    public string Channel { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTimeOffset? ReadAt { get; set; }
    public Guid? RelatedOrderId { get; set; }

    public User User { get; set; } = null!;
}
