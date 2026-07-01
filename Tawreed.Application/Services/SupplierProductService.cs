using Tawreed.Application.Common.Models;
using Tawreed.Application.Interfaces;
using Tawreed.Domain.Interfaces;

namespace Tawreed.Application.Services;

public class SupplierProductService : ISupplierProductService
{
    private readonly ISupplierProductRepository _supplierProductRepository;
    private readonly IPricingTierService _pricingTierService;
    private readonly IUnitOfWork _unitOfWork;

    public SupplierProductService(ISupplierProductRepository supplierProductRepository, IPricingTierService pricingTierService, IUnitOfWork unitOfWork)
    {
        _supplierProductRepository = supplierProductRepository;
        _pricingTierService = pricingTierService;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<SupplierProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _supplierProductRepository.GetAllAsync(cancellationToken);
        return items.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<SupplierProductDto>> GetBySupplierAsync(Guid supplierId, CancellationToken cancellationToken = default)
    {
        var items = await _supplierProductRepository.GetBySupplierAsync(supplierId, cancellationToken);
        return items.Select(MapToDto).ToList();
    }

    public async Task<SupplierProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _supplierProductRepository.GetWithDetailsAsync(id, cancellationToken);
        return item is null ? null : MapToDto(item);
    }

    public async Task<SupplierProductDto> AddProductAsync(Guid supplierId, Guid productId, decimal price, int stock, IReadOnlyList<CreatePricingTierRequest>? tiers = null, CancellationToken cancellationToken = default)
    {
        var entity = new Domain.Entities.SupplierProduct
        {
            SupplierId = supplierId,
            ProductId = productId,
            Price = price,
            Stock = stock,
            IsActive = true
        };
        _supplierProductRepository.Add(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (tiers?.Count > 0)
        {
            foreach (var tier in tiers)
            {
                await _pricingTierService.CreateAsync(entity.Id, tier, cancellationToken);
            }
        }

        var reloaded = await _supplierProductRepository.GetWithDetailsAsync(entity.Id, cancellationToken);
        return MapToDto(reloaded!);
    }

    public async Task<SupplierProductDto> UpdateProductAsync(Guid id, decimal? price, int? stock, bool? isActive, CancellationToken cancellationToken = default)
    {
        var entity = await _supplierProductRepository.GetWithDetailsAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException("Supplier product not found.");
        if (price.HasValue) entity.Price = price.Value;
        if (stock.HasValue) entity.Stock = stock.Value;
        if (isActive.HasValue) entity.IsActive = isActive.Value;
        _supplierProductRepository.Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(entity);
    }

    public async Task DeleteProductAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _supplierProductRepository.GetByIdAsync(id, cancellationToken);
        if (entity is not null)
        {
            _supplierProductRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    private static SupplierProductDto MapToDto(Domain.Entities.SupplierProduct sp)
    {
        var cover = sp.Images?.FirstOrDefault(i => i.IsCover) ?? sp.Images?.FirstOrDefault();
        return new SupplierProductDto(
            sp.Id, sp.SupplierId, sp.ProductId,
            sp.Product?.Name ?? "",
            sp.Product?.Description,
            sp.Product?.CategoryId ?? Guid.Empty,
            sp.Product?.Category?.NameAr ?? "",
            sp.Product?.UnitId ?? Guid.Empty,
            sp.Product?.Unit?.Symbol ?? "",
            cover?.ImageUrl,
            sp.Price, sp.Stock, sp.IsActive,
            sp.CreatedAt, sp.UpdatedAt);
    }
}
