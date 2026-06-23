using Tawreed.Domain.Common;

namespace Tawreed.Domain.Entities;

public class Category : BaseSoftDeletableEntity
{
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public string? IconUrl { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }

    public Category? Parent { get; set; }
    public ICollection<Category> Children { get; set; } = [];
    public ICollection<Product> Products { get; set; } = [];
    public ICollection<SupplierCategory> SupplierCategories { get; set; } = [];
}
