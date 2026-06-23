using Tawreed.Domain.Common;

namespace Tawreed.Domain.Entities;

public class ParticipantItem : BaseEntity
{
    public Guid ParticipantId { get; set; }
    public Guid GroupOrderItemId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPriceAtJoin { get; set; }
    public decimal? FinalUnitPrice { get; set; }
    public decimal? LineTotal { get; set; }

    public GroupOrderParticipant Participant { get; set; } = null!;
    public GroupOrderItem GroupOrderItem { get; set; } = null!;
}
