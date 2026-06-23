using Tawreed.Domain.Entities;

namespace Tawreed.Domain.Interfaces;

public interface IInvoiceRepository : IGenericRepository<Invoice>
{
    Task<IReadOnlyList<Invoice>> GetByBuyerAsync(Guid buyerId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Invoice>> GetByGroupOrderAsync(Guid groupOrderId, CancellationToken cancellationToken = default);
}
