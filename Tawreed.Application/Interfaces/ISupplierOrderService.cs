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
    public string? DeliveryPreference { get; set; }
    public Guid? PreferredDeliveryPersonId { get; set; }
    public string? PreferredDeliveryPersonName { get; set; }
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

public class AcceptOrderRequest
{
    public string? Notes { get; set; }
    public DateTimeOffset ScheduledDeliveryAt { get; set; }
    public string? DeliveryNotes { get; set; }
}

public class AvailableDeliveryPersonDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public double Rating { get; set; }
    public int TotalDeliveries { get; set; }
    public decimal BaseDeliveryFee { get; set; }
    public string? VehicleType { get; set; }
    public Guid? CoverageRegionId { get; set; }
    public string? CoverageRegionName { get; set; }
}

public class ProposeDeliveryFeeRequest
{
    public decimal Fee { get; set; }
    public string? Notes { get; set; }
}

public interface ISupplierOrderService
{
    Task<PaginatedResult<SupplierOrderListDto>> GetOrdersAsync(Guid userId, string? status = null, int page = 1, int limit = 20, CancellationToken cancellationToken = default);
    Task<object> AcceptOrderAsync(Guid orderId, Guid userId, AcceptOrderRequest request, CancellationToken cancellationToken = default);
    Task<object> DeclineOrderAsync(Guid orderId, Guid userId, string reason, CancellationToken cancellationToken = default);
    Task<PaginatedResult<DeliveryListDto>> GetDeliveriesAsync(Guid userId, string? status = null, int page = 1, int limit = 20, CancellationToken cancellationToken = default);
    Task<object> UpdateDeliveryStatusAsync(Guid deliveryId, string status, string? trackingNotes = null, DateTimeOffset? scheduledAt = null, CancellationToken cancellationToken = default);

    // Delivery person methods
    Task<IReadOnlyList<AvailableDeliveryPersonDto>> BrowseAvailableDeliveryPersonsAsync(Guid orderId, Guid userId, CancellationToken cancellationToken = default);
    Task<object> RequestDeliveryPersonAsync(Guid orderId, Guid deliveryPersonId, Guid userId, CancellationToken cancellationToken = default);
    Task<object> AssignDeliveryPersonAsync(Guid orderId, Guid deliveryPersonId, Guid userId, CancellationToken cancellationToken = default);
    Task<object> ProposeDeliveryFeeAsync(Guid orderId, decimal fee, string? notes, Guid userId, CancellationToken cancellationToken = default);
    Task<object> UseOwnDeliveryAsync(Guid orderId, Guid userId, CancellationToken cancellationToken = default);
    Task<object> CancelOrderFromSupplierAsync(Guid orderId, Guid userId, CancellationToken cancellationToken = default);
}
