using Tawreed.Domain.Common;

namespace Tawreed.Domain.Entities;

public class Unit : BaseEntity
{
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;

    public ICollection<Product> Products { get; set; } = [];
}
