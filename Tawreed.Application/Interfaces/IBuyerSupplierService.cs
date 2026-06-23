namespace Tawreed.Application.Interfaces;

public class SupplierPublicPricingTierDto
{
    public int MinQty { get; set; }
    public int? MaxQty { get; set; }
    public decimal UnitPrice { get; set; }
    public bool IsCurrentTier { get; set; }
}

public class SupplierPublicProductDto
{
    public Guid SupplierProductId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string CategoryId { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public int AvailableStock { get; set; }
    public bool IsRequiredByOrder { get; set; }
    public int? OrderRequestedQty { get; set; }
    public string? ImageUrl { get; set; }
    public List<SupplierPublicPricingTierDto> PricingTiers { get; set; } = [];
}

public class SupplierPublicProfileDto
{
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public decimal Rating { get; set; }
    public string? Address { get; set; }
    public List<string> Categories { get; set; } = [];
    public List<SupplierPublicProductDto> Products { get; set; } = [];
}

public interface IBuyerSupplierService
{
    Task<SupplierPublicProfileDto> GetSupplierProfileAsync(
        Guid supplierId,
        Guid orderId,
        CancellationToken cancellationToken = default);
}
