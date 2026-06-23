using Tawreed.Application.Common.Models;
using Tawreed.Domain.Entities;

namespace Tawreed.Application.Interfaces;

public interface ICategoryService
{
    Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CategoryDto>> GetRootAsync(CancellationToken cancellationToken = default);
    Task<CategoryDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CategoryDto> CreateAsync(Category category, CancellationToken cancellationToken = default);
    Task<CategoryDto> UpdateAsync(Category category, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
