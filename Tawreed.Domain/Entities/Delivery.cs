using Tawreed.Domain.Common;

namespace Tawreed.Domain.Entities;

public class Delivery : BaseAuditableEntity
{
    public Guid InvoiceId { get; set; }
    public Guid GroupOrderId { get; set; }
    public Guid SupplierId { get; set; }
    public Guid? DeliveryPersonId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset? ScheduledAt { get; set; }
    public DateTimeOffset? DeliveredAt { get; set; }
    public string? TrackingNotes { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public decimal? ShippingLatitude { get; set; }
    public decimal? ShippingLongitude { get; set; }

    public Invoice Invoice { get; set; } = null!;
    public GroupOrder GroupOrder { get; set; } = null!;
    public Supplier Supplier { get; set; } = null!;
    public User? DeliveryPerson { get; set; }
}
