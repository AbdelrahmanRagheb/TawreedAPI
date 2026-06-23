using Tawreed.Domain.Entities;
using Tawreed.Domain.Enums;

namespace Tawreed.Domain.Interfaces;

public interface IRegionRepository : IGenericRepository<Region>
{
    Task<IReadOnlyList<Region>> GetRootRegionsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Region>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<List<Guid>> GetDescendantIdsAsync(Guid regionId, CancellationToken cancellationToken = default);
    Task<List<Guid>> GetAncestorIdsAsync(Guid regionId, CancellationToken cancellationToken = default);
    Task<Region?> FindMatchingAncestorAsync(Guid regionId, List<RegionType> types, CancellationToken cancellationToken = default);
}
