using Tawreed.Domain.Common;

namespace Tawreed.Domain.Entities;

public class ProductImage : BaseEntity
{
    public Guid SupplierProductId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool IsCover { get; set; }

    public SupplierProduct SupplierProduct { get; set; } = null!;
}
