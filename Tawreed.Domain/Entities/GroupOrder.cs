using Tawreed.Domain.Common;

namespace Tawreed.Domain.Entities;

public class GroupOrder : BaseAuditableEntity
{
    public Guid CreatorId { get; set; }
    public Guid? SupplierId { get; set; }
    public Guid RegionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTimeOffset DeadlineAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset? ClosedAt { get; set; }

    public Buyer Creator { get; set; } = null!;
    public Supplier? Supplier { get; set; }
    public Region Region { get; set; } = null!;
    public ICollection<GroupOrderItem> Items { get; set; } = [];
    public ICollection<GroupOrderParticipant> Participants { get; set; } = [];
    public ICollection<GroupOrderEvent> Events { get; set; } = [];
    public ICollection<Invoice> Invoices { get; set; } = [];
    public ICollection<Delivery> Deliveries { get; set; } = [];
}
