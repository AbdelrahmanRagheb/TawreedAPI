using Tawreed.Domain.Entities;

namespace Tawreed.Domain.Interfaces;

public interface ISupplierApprovalLogRepository : IGenericRepository<SupplierApprovalLog>
{
    Task<IReadOnlyList<SupplierApprovalLog>> GetBySupplierAsync(Guid supplierId, CancellationToken cancellationToken = default);
}
