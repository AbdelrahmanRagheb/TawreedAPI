using Tawreed.Application.Common.Models;
using Tawreed.Application.Interfaces;
using Tawreed.Domain.Entities;
using Tawreed.Domain.Interfaces;

namespace Tawreed.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        return categories.Select(c => new CategoryDto(c.Id, c.NameAr, c.NameEn, c.ParentId, c.IconUrl, c.SortOrder, c.IsActive)).ToList();
    }

    public async Task<IReadOnlyList<CategoryDto>> GetRootAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetRootCategoriesAsync(cancellationToken);
        return categories.Select(c => new CategoryDto(c.Id, c.NameAr, c.NameEn, c.ParentId, c.IconUrl, c.SortOrder, c.IsActive)).ToList();
    }

    public async Task<CategoryDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var c = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        return c is null ? null : new CategoryDto(c.Id, c.NameAr, c.NameEn, c.ParentId, c.IconUrl, c.SortOrder, c.IsActive);
    }

    public async Task<CategoryDto> CreateAsync(Category category, CancellationToken cancellationToken = default)
    {
        _categoryRepository.Add(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new CategoryDto(category.Id, category.NameAr, category.NameEn, category.ParentId, category.IconUrl, category.SortOrder, category.IsActive);
    }

    public async Task<CategoryDto> UpdateAsync(Category category, CancellationToken cancellationToken = default)
    {
        _categoryRepository.Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new CategoryDto(category.Id, category.NameAr, category.NameEn, category.ParentId, category.IconUrl, category.SortOrder, category.IsActive);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category is not null)
        {
            _categoryRepository.SoftDelete(category);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
