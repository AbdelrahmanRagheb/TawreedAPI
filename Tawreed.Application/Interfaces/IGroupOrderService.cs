using Tawreed.Application.Common.Models;
using Tawreed.Domain.Entities;

namespace Tawreed.Application.Interfaces;

public interface IGroupOrderService
{
    Task<IReadOnlyList<GroupOrderDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<GroupOrderDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<GroupOrderDto> CreateAsync(GroupOrder groupOrder, CancellationToken cancellationToken = default);
    Task<GroupOrderDto> UpdateStatusAsync(Guid id, string status, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
