using Tawreed.Application.Common.Models;
using Tawreed.Application.Interfaces;
using Tawreed.Domain.Interfaces;

namespace Tawreed.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetAllWithCategoryAsync(cancellationToken);
        return products.Select(p => MapToDto(p)).ToList();
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var p = await _productRepository.GetWithCategoryAsync(id, cancellationToken);
        return p is null ? null : MapToDto(p);
    }

    public async Task<ProductDto> CreateAsync(Domain.Entities.Product product, CancellationToken cancellationToken = default)
    {
        _productRepository.Add(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(product);
    }

    public async Task<ProductDto> UpdateAsync(Domain.Entities.Product product, CancellationToken cancellationToken = default)
    {
        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(product);
    }

    private static ProductDto MapToDto(Domain.Entities.Product p)
    {
        return new ProductDto(
            p.Id, p.Name, p.Description,
            p.CategoryId, p.Category?.NameEn ?? "",
            p.UnitId, p.Unit?.Symbol ?? "",
            p.CreatedAt, p.UpdatedAt);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product is not null)
        {
            _productRepository.Delete(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
