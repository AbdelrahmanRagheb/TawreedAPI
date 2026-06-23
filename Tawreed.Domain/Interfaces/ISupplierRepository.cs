using Tawreed.Domain.Entities;

namespace Tawreed.Domain.Interfaces;

public interface ISupplierRepository : IGenericRepository<Supplier>
{
    Task<Supplier?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Supplier?> GetByIdWithDetailsAsync(Guid supplierId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Supplier>> GetApprovedAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Supplier>> GetPendingApprovalAsync(CancellationToken cancellationToken = default);
}
