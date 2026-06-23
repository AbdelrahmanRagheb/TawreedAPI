using Tawreed.Application.Interfaces;
using Tawreed.Domain.Interfaces;

namespace Tawreed.Application.Services;

public class BuyerSupplierService : IBuyerSupplierService
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly ISupplierProductRepository _supplierProductRepository;
    private readonly IGroupOrderRepository _groupOrderRepository;

    public BuyerSupplierService(
        ISupplierRepository supplierRepository,
        ISupplierProductRepository supplierProductRepository,
        IGroupOrderRepository groupOrderRepository)
    {
        _supplierRepository = supplierRepository;
        _supplierProductRepository = supplierProductRepository;
        _groupOrderRepository = groupOrderRepository;
    }

    public async Task<SupplierPublicProfileDto> GetSupplierProfileAsync(
        Guid supplierId,
        Guid orderId,
        CancellationToken cancellationToken = default)
    {
        // Load supplier with User, Region, SupplierCategories for the profile header
        var supplierFull = await _supplierRepository.GetByIdWithDetailsAsync(supplierId, cancellationToken)
            ?? throw new KeyNotFoundException("Supplier not found or not approved.");

        // Load the order to know which products are requested and their quantities
        var order = await _groupOrderRepository.GetWithDetailsAsync(orderId, cancellationToken);
        var orderProductMap = order?.Items?
            .Where(i => i.Product != null)
            .ToDictionary(i => i.ProductId, i => i.TargetQty)
            ?? new Dictionary<Guid, int>();

        // Load all active products this supplier offers with tiers and images
        var supplierProducts = await _supplierProductRepository.GetBySupplierWithTiersAsync(supplierId, cancellationToken);

        // Build unique category names from the supplier's active products
        var categories = supplierProducts
            .Where(sp => sp.Product?.Category != null)
            .Select(sp => sp.Product!.Category!.NameEn)
            .Distinct()
            .OrderBy(c => c)
            .ToList();

        // Map products, sorting required products to the top
        var productDtos = supplierProducts
            .Select(sp =>
            {
                bool isRequired = orderProductMap.ContainsKey(sp.ProductId);
                int requestedQty = isRequired ? orderProductMap[sp.ProductId] : 0;

                var tiers = (sp.PricingTiers ?? [])
                    .OrderBy(t => t.MinQty)
                    .Select(t => new SupplierPublicPricingTierDto
                    {
                        MinQty = t.MinQty,
                        MaxQty = t.MaxQty,
                        UnitPrice = t.UnitPrice,
                        IsCurrentTier = isRequired &&
                            requestedQty >= t.MinQty &&
                            (t.MaxQty == null || requestedQty <= t.MaxQty)
                    })
                    .ToList();

                var coverImage = sp.Images?.FirstOrDefault(i => i.IsCover) ?? sp.Images?.FirstOrDefault();

                return new SupplierPublicProductDto
                {
                    SupplierProductId = sp.Id,
                    ProductId = sp.ProductId,
                    ProductName = sp.Product?.Name ?? "",
                    CategoryId = (sp.Product?.CategoryId ?? Guid.Empty).ToString(),
                    CategoryName = sp.Product?.Category?.NameEn ?? "",
                    Unit = sp.Product?.Unit?.Symbol ?? "",
                    BasePrice = sp.Price,
                    AvailableStock = sp.Stock,
                    IsRequiredByOrder = isRequired,
                    OrderRequestedQty = isRequired ? requestedQty : null,
                    ImageUrl = coverImage?.ImageUrl,
                    PricingTiers = tiers
                };
            })
            // Required products first, then alphabetically
            .OrderByDescending(p => p.IsRequiredByOrder)
            .ThenBy(p => p.ProductName)
            .ToList();

        return new SupplierPublicProfileDto
        {
            SupplierId = supplierFull.Id,
            SupplierName = supplierFull.CompanyName,
            Rating = supplierFull.RatingAvg,
            Address = supplierFull.Address,
            Categories = categories,
            Products = productDtos
        };
    }
}
