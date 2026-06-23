using Microsoft.EntityFrameworkCore;
using Tawreed.Domain.Entities;
using Tawreed.Domain.Enums;
using Tawreed.Domain.Interfaces;
using Tawreed.Infrastructure.Data;

namespace Tawreed.Infrastructure.Repositories;

public class RegionRepository : GenericRepository<Region>, IRegionRepository
{
    public RegionRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Region>> GetRootRegionsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(r => r.ParentId == null).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Region>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(r => r.IsActive).ToListAsync(cancellationToken);
    }

    public async Task<List<Guid>> GetDescendantIdsAsync(Guid regionId, CancellationToken cancellationToken = default)
    {
        var all = await DbSet.ToListAsync(cancellationToken);
        var result = new List<Guid>();
        CollectDescendantIds(all, regionId, result);
        return result;
    }

    public async Task<List<Guid>> GetAncestorIdsAsync(Guid regionId, CancellationToken cancellationToken = default)
    {
        var all = await DbSet.ToListAsync(cancellationToken);
        var lookup = all.ToDictionary(r => r.Id);
        var result = new List<Guid>();
        var current = regionId;
        while (lookup.TryGetValue(current, out var region) && region.ParentId.HasValue)
        {
            result.Add(region.ParentId.Value);
            current = region.ParentId.Value;
        }
        return result;
    }

    public async Task<Region?> FindMatchingAncestorAsync(Guid regionId, List<RegionType> types, CancellationToken cancellationToken = default)
    {
        var all = await DbSet.ToListAsync(cancellationToken);
        var lookup = all.ToDictionary(r => r.Id);

        var current = regionId;
        while (lookup.TryGetValue(current, out var region))
        {
            if (types.Contains(region.Type))
                return region;
            if (!region.ParentId.HasValue)
                break;
            current = region.ParentId.Value;
        }
        return null;
    }

    private static void CollectDescendantIds(List<Region> all, Guid parentId, List<Guid> result)
    {
        var children = all.Where(r => r.ParentId == parentId).ToList();
        foreach (var child in children)
        {
            result.Add(child.Id);
            CollectDescendantIds(all, child.Id, result);
        }
    }
}