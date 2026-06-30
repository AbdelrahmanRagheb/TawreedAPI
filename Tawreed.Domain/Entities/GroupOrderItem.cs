using Tawreed.Domain.Common;

namespace Tawreed.Domain.Entities;

public class GroupOrderItem : BaseAuditableEntity
{
    public Guid GroupOrderId { get; set; }
    public Guid ProductId { get; set; }
    public int TargetQty { get; set; }
    public Guid? SupplierProductId { get; set; }
    public decimal? UnitPrice { get; set; }
    public Guid? SupplierId { get; set; }
    public string ItemStatus { get; set; } = "Unassigned";

    public GroupOrder GroupOrder { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public SupplierProduct? SupplierProduct { get; set; }
    public Supplier? Supplier { get; set; }
    public ICollection<ParticipantItem> ParticipantItems { get; set; } = [];
}
