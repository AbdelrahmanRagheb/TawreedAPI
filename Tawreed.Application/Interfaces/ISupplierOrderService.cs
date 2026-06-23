using Tawreed.Domain.Entities;

namespace Tawreed.Application.Interfaces;

public class SupplierOrderListDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string CreatorName { get; set; } = string.Empty;
    public string? BuyerCompany { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset Deadline { get; set; }
    public string Region { get; set; } = string.Empty;
    public DateTimeOffset ReceivedAt { get; set; }
    public required List<SupplierOrderItemDto> Items { get; set; }
}

public class SupplierOrderItemDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}

public class DeliveryListDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset? ScheduledAt { get; set; }
    public DateTimeOffset? DeliveredAt { get; set; }
    public string? TrackingNotes { get; set; }
    public string BuyerName { get; set; } = string.Empty;
    public required List<DeliveryItemDto> Items { get; set; }
}

public class DeliveryItemDto
{
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
}

public interface ISupplierOrderService
{
    Task<PaginatedResult<SupplierOrderListDto>> GetOrdersAsync(Guid userId, string? status = null, int page = 1, int limit = 20, CancellationToken cancellationToken = default);
    Task<object> AcceptOrderAsync(Guid orderId, Guid userId, string? notes = null, CancellationToken cancellationToken = default);
    Task<object> DeclineOrderAsync(Guid orderId, Guid userId, string reason, CancellationToken cancellationToken = default);
    Task<PaginatedResult<DeliveryListDto>> GetDeliveriesAsync(Guid userId, string? status = null, int page = 1, int limit = 20, CancellationToken cancellationToken = default);
    Task<object> UpdateDeliveryStatusAsync(Guid deliveryId, string status, string? trackingNotes = null, DateTimeOffset? scheduledAt = null, CancellationToken cancellationToken = default);
}
