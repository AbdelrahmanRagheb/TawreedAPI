using Tawreed.Domain.Common;

namespace Tawreed.Domain.Entities;

public class Buyer : BaseAuditableEntity
{
    public Guid UserId { get; set; }
    public string BusinessName { get; set; } = string.Empty;
    public string BusinessType { get; set; } = string.Empty;
    public string? TaxId { get; set; }
    public Guid RegionId { get; set; }
    public string? Address { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public decimal RatingAvg { get; set; }

    public User User { get; set; } = null!;
    public Region Region { get; set; } = null!;
    public ICollection<GroupOrder> CreatedGroupOrders { get; set; } = [];
    public ICollection<GroupOrderParticipant> Participants { get; set; } = [];
    public ICollection<Invoice> Invoices { get; set; } = [];
}
