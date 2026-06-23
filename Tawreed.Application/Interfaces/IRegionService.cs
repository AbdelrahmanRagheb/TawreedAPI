using Tawreed.Application.Common.Models;

namespace Tawreed.Application.Interfaces;

public interface IRegionService
{
    Task<IReadOnlyList<RegionDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RegionDto>> GetRootAsync(CancellationToken cancellationToken = default);
    Task<RegionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RegionDto>> GetChildrenAsync(Guid parentId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RegionSearchDto>> SearchAsync(string query, CancellationToken cancellationToken = default);
}
