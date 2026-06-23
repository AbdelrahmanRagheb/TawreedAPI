using Tawreed.Application.Common.Models;
using Tawreed.Application.Interfaces;
using Tawreed.Domain.Interfaces;

namespace Tawreed.Application.Services;

public class PricingTierService : IPricingTierService
{
    private readonly IPricingTierRepository _pricingTierRepository;
    private readonly ISupplierProductRepository _supplierProductRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PricingTierService(
        IPricingTierRepository pricingTierRepository,
        ISupplierProductRepository supplierProductRepository,
        IUnitOfWork unitOfWork)
    {
        _pricingTierRepository = pricingTierRepository;
        _supplierProductRepository = supplierProductRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<PricingTierDto>> GetByProductAsync(Guid supplierProductId, CancellationToken cancellationToken = default)
    {
        var tiers = await _pricingTierRepository.GetBySupplierProductAsync(supplierProductId, cancellationToken);
        return tiers.Select(MapToDto).ToList();
    }

    public async Task<PricingTierDto> CreateAsync(Guid supplierProductId, CreatePricingTierRequest request, CancellationToken cancellationToken = default)
    {
        var product = await _supplierProductRepository.GetByIdAsync(supplierProductId, cancellationToken)
            ?? throw new KeyNotFoundException("Product not found.");

        var entity = new Domain.Entities.PricingTier
        {
            Id = Guid.NewGuid(),
            SupplierProductId = supplierProductId,
            MinQty = request.MinQty,
            MaxQty = request.MaxQty,
            UnitPrice = request.UnitPrice
        };

        _pricingTierRepository.Add(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(entity);
    }

    public async Task<PricingTierDto> UpdateAsync(Guid tierId, UpdatePricingTierRequest request, CancellationToken cancellationToken = default)
    {
        var tier = await _pricingTierRepository.GetByIdAsync(tierId, cancellationToken)
            ?? throw new KeyNotFoundException("Pricing tier not found.");

        tier.MinQty = request.MinQty;
        tier.MaxQty = request.MaxQty;
        tier.UnitPrice = request.UnitPrice;

        _pricingTierRepository.Update(tier);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(tier);
    }

    public async Task DeleteAsync(Guid tierId, CancellationToken cancellationToken = default)
    {
        var tier = await _pricingTierRepository.GetByIdAsync(tierId, cancellationToken)
            ?? throw new KeyNotFoundException("Pricing tier not found.");

        _pricingTierRepository.Delete(tier);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static PricingTierDto MapToDto(Domain.Entities.PricingTier tier) => new(
        tier.Id,
        tier.SupplierProductId,
        tier.MinQty,
        tier.MaxQty,
        tier.UnitPrice);
}
