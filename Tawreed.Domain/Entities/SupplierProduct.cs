using Tawreed.Domain.Common;

namespace Tawreed.Domain.Entities;

public class SupplierProduct : BaseAuditableEntity
{
    public Guid SupplierId { get; set; }
    public Guid ProductId { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsActive { get; set; }

    public Supplier Supplier { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public ICollection<ProductImage> Images { get; set; } = [];
    public ICollection<PricingTier> PricingTiers { get; set; } = [];
    public ICollection<GroupOrderItem> GroupOrderItems { get; set; } = [];
}
