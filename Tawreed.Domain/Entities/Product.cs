using Tawreed.Domain.Common;

namespace Tawreed.Domain.Entities;

public class Product : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public Guid UnitId { get; set; }
    public decimal? MarketPrice { get; set; }

    public Category Category { get; set; } = null!;
    public Unit Unit { get; set; } = null!;
    public ICollection<SupplierProduct> SupplierProducts { get; set; } = [];
    public ICollection<GroupOrderItem> GroupOrderItems { get; set; } = [];
}
