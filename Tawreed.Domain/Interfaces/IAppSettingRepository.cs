using Tawreed.Domain.Entities;

namespace Tawreed.Domain.Interfaces;

public interface IAppSettingRepository : IGenericRepository<AppSetting>
{
    Task<AppSetting?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
}
