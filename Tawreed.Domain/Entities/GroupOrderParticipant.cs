using Tawreed.Domain.Common;

namespace Tawreed.Domain.Entities;

public class GroupOrderParticipant : BaseEntity
{
    public Guid GroupOrderId { get; set; }
    public Guid BuyerId { get; set; }
    public DateTimeOffset JoinedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset? CancelledAt { get; set; }

    public GroupOrder GroupOrder { get; set; } = null!;
    public Buyer Buyer { get; set; } = null!;
    public ICollection<ParticipantItem> Items { get; set; } = [];
    public ICollection<Invoice> Invoices { get; set; } = [];
}
