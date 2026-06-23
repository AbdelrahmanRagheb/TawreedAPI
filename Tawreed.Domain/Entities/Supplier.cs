using Tawreed.Domain.Common;

namespace Tawreed.Domain.Entities;

public class Supplier : BaseAuditableEntity
{
    public Guid UserId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? TaxId { get; set; }
    public Guid RegionId { get; set; }
    public bool IsApproved { get; set; }
    public Guid? ApprovedBy { get; set; }
    public DateTimeOffset? ApprovedAt { get; set; }
    public decimal RatingAvg { get; set; }
    public string? Address { get; set; }

    public User User { get; set; } = null!;
    public Region Region { get; set; } = null!;
    public User? Approver { get; set; }
    public ICollection<SupplierProduct> SupplierProducts { get; set; } = [];
    public ICollection<SupplierCategory> SupplierCategories { get; set; } = [];
    public ICollection<SupplierApprovalLog> ApprovalLogs { get; set; } = [];
    public ICollection<GroupOrder> GroupOrders { get; set; } = [];
    public ICollection<Delivery> Deliveries { get; set; } = [];
}
