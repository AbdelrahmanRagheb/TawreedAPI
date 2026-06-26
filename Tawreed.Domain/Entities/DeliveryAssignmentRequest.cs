using Tawreed.Domain.Common;

namespace Tawreed.Domain.Entities;

public class DeliveryAssignmentRequest : BaseAuditableEntity
{
    public Guid OrderId { get; set; }
    public Guid DeliveryPersonId { get; set; }
    public Guid SupplierId { get; set; }
    public string Status { get; set; } = "Pending";
    public decimal? ProposedFee { get; set; }
    public DateTimeOffset? RespondedAt { get; set; }
    public string? DeclineReason { get; set; }

    public GroupOrder Order { get; set; } = null!;
    public DeliveryPersonProfile DeliveryPerson { get; set; } = null!;
    public Supplier Supplier { get; set; } = null!;
}
