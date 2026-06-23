using Tawreed.Domain.Common;

namespace Tawreed.Domain.Entities;

public class Invoice : BaseEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid GroupOrderId { get; set; }
    public Guid BuyerId { get; set; }
    public Guid ParticipantId { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal Total { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public DateTimeOffset IssuedAt { get; set; }
    public DateTimeOffset? PaidAt { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public decimal? ShippingLatitude { get; set; }
    public decimal? ShippingLongitude { get; set; }

    public GroupOrder GroupOrder { get; set; } = null!;
    public Buyer Buyer { get; set; } = null!;
    public GroupOrderParticipant Participant { get; set; } = null!;
    public Delivery? Delivery { get; set; }
}
