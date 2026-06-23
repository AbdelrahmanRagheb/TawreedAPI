namespace Tawreed.Application.Common.Models;

public record PricingTierDto(
    Guid Id,
    Guid SupplierProductId,
    int MinQty,
    int? MaxQty,
    decimal UnitPrice);

public record CreatePricingTierRequest(
    int MinQty,
    int? MaxQty,
    decimal UnitPrice);

public record UpdatePricingTierRequest(
    int MinQty,
    int? MaxQty,
    decimal UnitPrice);
