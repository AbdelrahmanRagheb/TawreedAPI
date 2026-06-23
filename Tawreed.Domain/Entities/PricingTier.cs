using Tawreed.Domain.Common;

namespace Tawreed.Domain.Entities;

public class PricingTier : BaseEntity
{
    public Guid SupplierProductId { get; set; }
    public int MinQty { get; set; }
    public int? MaxQty { get; set; }
    public decimal UnitPrice { get; set; }

    public SupplierProduct SupplierProduct { get; set; } = null!;
}
