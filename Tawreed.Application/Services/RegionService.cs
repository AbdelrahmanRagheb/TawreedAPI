using Tawreed.Application.Common.Models;
using Tawreed.Application.Interfaces;
using Tawreed.Domain.Entities;
using Tawreed.Domain.Interfaces;

namespace Tawreed.Application.Services;

public class RegionService : IRegionService
{
    private readonly IRegionRepository _regionRepository;

    public RegionService(IRegionRepository regionRepository)
    {
        _regionRepository = regionRepository;
    }

    public async Task<IReadOnlyList<RegionDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var regions = await _regionRepository.GetActiveAsync(cancellationToken);
        return regions.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<RegionDto>> GetRootAsync(CancellationToken cancellationToken = default)
    {
        var regions = await _regionRepository.GetRootRegionsAsync(cancellationToken);
        return regions.Select(MapToDto).ToList();
    }

    public async Task<RegionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var r = await _regionRepository.GetByIdAsync(id, cancellationToken);
        return r is null ? null : MapToDto(r);
    }

    public async Task<IReadOnlyList<RegionDto>> GetChildrenAsync(Guid parentId, CancellationToken cancellationToken = default)
    {
        var regions = await _regionRepository.FindAsync(r => r.ParentId == parentId && r.IsActive, cancellationToken);
        return regions.OrderBy(r => r.NameAr).Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<RegionSearchDto>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
            return [];

        var regions = await _regionRepository.FindAsync(
            r => r.IsActive && (r.NameAr.Contains(query) || r.NameEn.Contains(query)),
            cancellationToken);

        return regions.Select(r => new RegionSearchDto(r.Id, r.NameAr, r.NameEn)).ToList();
    }

    private static RegionDto MapToDto(Region r) =>
        new(r.Id, r.NameAr, r.NameEn, r.ParentId, r.Type, r.IsActive, r.CreatedAt, r.UpdatedAt);
}
