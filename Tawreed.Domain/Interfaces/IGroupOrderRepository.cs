using Tawreed.Domain.Entities;

namespace Tawreed.Domain.Interfaces;

public interface IGroupOrderRepository : IGenericRepository<GroupOrder>
{
    Task<IReadOnlyList<GroupOrder>> GetByCreatorAsync(Guid creatorId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GroupOrder>> GetBySupplierAsync(Guid supplierId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GroupOrder>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
    Task<GroupOrder?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<GroupOrder>> GetAllWithAdminDetailsAsync(CancellationToken cancellationToken = default);
}
