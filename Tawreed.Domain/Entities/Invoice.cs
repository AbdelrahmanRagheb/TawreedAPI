using Tawreed.Domain.Common;

namespace Tawreed.Domain.Entities;

public class Invoice : BaseEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid GroupOrderId { get; set; }
    public Guid BuyerId { get; set; }
    public Guid? ParticipantId { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal Total { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public DateTimeOffset? PaidAt { get; set; }
    public string VerificationCode { get; set; } = string.Empty;
    public string ShippingRegion { get; set; } = string.Empty;

    public GroupOrder GroupOrder { get; set; } = null!;
    public Buyer Buyer { get; set; } = null!;
    public GroupOrderParticipant? Participant { get; set; }
}
