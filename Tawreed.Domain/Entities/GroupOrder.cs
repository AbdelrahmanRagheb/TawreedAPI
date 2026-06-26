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
    public string? Notes { get; set; }
    public string? Visibility { get; set; }
    public DateTimeOffset DeadlineAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset? ClosedAt { get; set; }

    // Added for delivery person assignment
    public string DeliveryPreference { get; set; } = "None";
    public Guid? PreferredDeliveryPersonId { get; set; }
    public Guid? AssignedDeliveryPersonId { get; set; }
    public decimal? ProposedDeliveryFee { get; set; }
    public string? DeliveryApprovalStatus { get; set; }

    public DeliveryPersonProfile? AssignedDeliveryPerson { get; set; }
    public DeliveryPersonProfile? PreferredDeliveryPerson { get; set; }
    public Buyer Creator { get; set; } = null!;
    public Supplier? Supplier { get; set; }
    public Region Region { get; set; } = null!;
    public ICollection<GroupOrderItem> Items { get; set; } = [];
    public ICollection<GroupOrderParticipant> Participants { get; set; } = [];
    public ICollection<GroupOrderEvent> Events { get; set; } = [];
    public ICollection<Invoice> Invoices { get; set; } = [];
    public ICollection<Delivery> Deliveries { get; set; } = [];
}
