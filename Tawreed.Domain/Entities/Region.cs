using Tawreed.Domain.Common;
using Tawreed.Domain.Enums;

namespace Tawreed.Domain.Entities;

public class Region : BaseAuditableEntity
{
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public RegionType Type { get; set; }
    public bool IsActive { get; set; }

    public Region? Parent { get; set; }
    public ICollection<Region> Children { get; set; } = [];
    public ICollection<Buyer> Buyers { get; set; } = [];
    public ICollection<Supplier> Suppliers { get; set; } = [];
    public ICollection<GroupOrder> GroupOrders { get; set; } = [];
}
