namespace Tawreed.Domain.Entities;

public class SupplierCategory
{
    public Guid SupplierId { get; set; }
    public Guid CategoryId { get; set; }

    public Supplier Supplier { get; set; } = null!;
    public Category Category { get; set; } = null!;
}
