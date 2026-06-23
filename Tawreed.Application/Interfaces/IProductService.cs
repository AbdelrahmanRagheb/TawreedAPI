using Tawreed.Application.Common.Models;
using Tawreed.Domain.Entities;

namespace Tawreed.Application.Interfaces;

public interface IProductService
{
    Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ProductDto> CreateAsync(Product product, CancellationToken cancellationToken = default);
    Task<ProductDto> UpdateAsync(Product product, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
