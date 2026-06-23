using Microsoft.EntityFrameworkCore;
using Tawreed.Domain.Entities;
using Tawreed.Domain.Interfaces;
using Tawreed.Infrastructure.Data;

namespace Tawreed.Infrastructure.Repositories;

public class AppSettingRepository : GenericRepository<AppSetting>, IAppSettingRepository
{
    public AppSettingRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<AppSetting?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(s => s.Key == key, cancellationToken);
    }
}
