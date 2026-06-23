using Tawreed.Domain.Common;

namespace Tawreed.Domain.Entities;

public class GroupOrderEvent : BaseAuditableEntity
{
    public Guid GroupOrderId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string? NotesAr { get; set; }
    public string? NotesEn { get; set; }
    public Guid CreatedBy { get; set; }

    public GroupOrder GroupOrder { get; set; } = null!;
    public User Creator { get; set; } = null!;
}
