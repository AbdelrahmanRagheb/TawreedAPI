using Tawreed.Application.Common.Models;
using Tawreed.Domain.Entities;

namespace Tawreed.Application.Interfaces;

public class OrderListDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset Deadline { get; set; }
    public decimal TotalOrderValue { get; set; }
    public int ParticipantCount { get; set; }
    public int ProductCount { get; set; }
    public string Region { get; set; } = string.Empty;
    public string CreatorName { get; set; } = string.Empty;
    public Guid CreatorId { get; set; }
    public bool IsCreator { get; set; }
    public bool IsParticipant { get; set; }
}

public class PaginatedResult<T>
{
    public required IReadOnlyList<T> Items { get; set; }
    public int Page { get; set; }
    public int Limit { get; set; }
    public int Total { get; set; }
    public int TotalPages { get; set; }
}

public class OrderDetailDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid CreatorId { get; set; }
    public Guid CreatorUserId { get; set; }
    public string CreatorName { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset Deadline { get; set; }
    public bool DeadlinePassed { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalOrderValue { get; set; }
    public int? TotalProductCount { get; set; }
    public int? AssignedProductCount { get; set; }
    public string? DeliveryPreference { get; set; }
    public string? PreferredDeliveryPersonName { get; set; }
    public decimal? ProposedDeliveryFee { get; set; }
    public string? DeliveryApprovalStatus { get; set; }
    public string? AssignedDeliveryPersonName { get; set; }
    public required List<OrderProductDto> Products { get; set; }
    public required List<OrderParticipantDto> Participants { get; set; }
    public required List<OrderActivityDto> Activities { get; set; }
    public bool IsParticipant { get; set; }
}

public class OrderProductDto
{
    public Guid GroupOrderItemId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public int CurrentQuantity { get; set; }
    public int TargetQuantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal? UnitPrice { get; set; }
    public decimal? MarketPrice { get; set; }
    public Guid? SupplierProductId { get; set; }
    public Guid? SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string ItemStatus { get; set; } = "Unassigned";
}

public class OrderParticipantDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTimeOffset JoinedAt { get; set; }
    public required List<ParticipantItemDto> Items { get; set; }
}

public class ParticipantItemDto
{
    public Guid GroupOrderItemId { get; set; }
    public Guid SupplierProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
}

public class OrderActivityDto
{
    public Guid Id { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string? NotesAr { get; set; }
    public string? NotesEn { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}

public class JoinOrderRequest
{
    public required List<JoinItem> Items { get; set; }
}

public class JoinItem
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}

public class UpdateItem
{
    public Guid GroupOrderItemId { get; set; }
    public int Quantity { get; set; }
}

public class UpdateItemsRequest
{
    public required List<UpdateItem> Items { get; set; }
}

public class CreateOrderRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsUrgent { get; set; }
    public required List<CreateOrderItem> Items { get; set; }
}

public class CreateOrderItem
{
    public Guid ProductId { get; set; }
    public int TargetQuantity { get; set; }
}

public class EligibleSupplierDto
{
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public decimal TotalEstimatedCost { get; set; }
    public int CoveredProductCount { get; set; }
    public required List<EligibleProductDto> CoveredProducts { get; set; }
}

public class EligibleProductDto
{
    public Guid ProductId { get; set; }
    public Guid GroupOrderItemId { get; set; }
    public decimal UnitPrice { get; set; }
    public int AvailableStock { get; set; }
    public required List<EligiblePricingTierDto> PricingTiers { get; set; }
}

public class EligiblePricingTierDto
{
    public int MinQty { get; set; }
    public int? MaxQty { get; set; }
    public decimal UnitPrice { get; set; }
}

public class AssignSupplierItemsRequest
{
    public Guid SupplierId { get; set; }
    public required List<Guid> ItemIds { get; set; }
}

public class BuyerDeliveryItemDto
{
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
}

public class BuyerDeliveryDto
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string OrderTitle { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset? ScheduledAt { get; set; }
    public string? DeliveryPersonName { get; set; }
    public string VerificationCode { get; set; } = string.Empty;
    public string ShippingRegion { get; set; } = string.Empty;
    public List<BuyerDeliveryItemDto> Items { get; set; } = [];
}

public interface IBuyerOrderService
{
    Task<PaginatedResult<OrderListDto>> GetOrdersAsync(Guid userId, string? status = null, int page = 1, int limit = 20, CancellationToken cancellationToken = default);
    Task<OrderDetailDto> GetOrderDetailAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<GroupOrderDto> CreateOrderAsync(Guid userId, CreateOrderRequest request, CancellationToken cancellationToken = default);
    Task<GroupOrderDto> SaveDraftAsync(Guid userId, CreateOrderRequest request, CancellationToken cancellationToken = default);
    Task<PaginatedResult<OrderListDto>> GetDraftsAsync(Guid userId, int page = 1, int limit = 20, CancellationToken cancellationToken = default);
    Task DeleteDraftAsync(Guid orderId, Guid userId, CancellationToken cancellationToken = default);
    Task<object> JoinOrderAsync(Guid orderId, Guid userId, JoinOrderRequest request, CancellationToken cancellationToken = default);
    Task<object> LeaveOrderAsync(Guid orderId, Guid userId, CancellationToken cancellationToken = default);
    Task<object> UpdateItemsAsync(Guid orderId, Guid participantId, Guid userId, UpdateItemsRequest request, CancellationToken cancellationToken = default);
    Task<object> UpdateOrderItemsAsync(Guid orderId, Guid userId, List<CreateOrderItem> items, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EligibleSupplierDto>> GetEligibleSuppliersAsync(Guid orderId, Guid userId, CancellationToken cancellationToken = default);
    Task<object> AssignSupplierItemsAsync(Guid orderId, AssignSupplierItemsRequest request, Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<BuyerDeliveryDto>> GetMyDeliveriesAsync(Guid userId, CancellationToken cancellationToken = default);

    // Delivery person methods (system-assigned, kept for backward compatibility)
    Task<object> SetDeliveryPreferenceAsync(Guid orderId, string preference, Guid? preferredDeliveryPersonId, Guid userId, CancellationToken cancellationToken = default);
    Task<object> ApproveDeliveryFeeAsync(Guid orderId, bool isApproved, string? reason, Guid userId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DeliveryPersonProfileDto>> GetAvailableDeliveryPersonsAsync(Guid orderId, CancellationToken cancellationToken = default);
}
