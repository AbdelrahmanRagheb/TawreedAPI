using Tawreed.Application.Common.Models;
using Tawreed.Application.Interfaces;
using Tawreed.Domain.Entities;
using Tawreed.Domain.Enums;
using Tawreed.Domain.Interfaces;

namespace Tawreed.Application.Services;

public class SupplierOrderService : ISupplierOrderService
{
    private readonly IGroupOrderRepository _groupOrderRepository;
    private readonly IGroupOrderEventRepository _eventRepository;
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly IRegionRepository _regionRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SupplierOrderService(
        IGroupOrderRepository groupOrderRepository,
        IGroupOrderEventRepository eventRepository,
        IDeliveryRepository deliveryRepository,
        ISupplierRepository supplierRepository,
        IRegionRepository regionRepository,
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _groupOrderRepository = groupOrderRepository;
        _eventRepository = eventRepository;
        _deliveryRepository = deliveryRepository;
        _supplierRepository = supplierRepository;
        _regionRepository = regionRepository;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedResult<SupplierOrderListDto>> GetOrdersAsync(Guid userId, string? status = null, int page = 1, int limit = 20, CancellationToken cancellationToken = default)
    {
        var supplier = await _supplierRepository.GetByUserIdAsync(userId, cancellationToken);
        if (supplier == null) return new PaginatedResult<SupplierOrderListDto> { Items = [], Page = page, Limit = limit, Total = 0, TotalPages = 0 };
        if (!supplier.IsApproved)
            throw new InvalidOperationException("Account pending approval.");

        var assignedOrders = await _groupOrderRepository.GetBySupplierAsync(supplier.Id, cancellationToken);
        var assignedIds = assignedOrders.Select(o => o.Id).ToHashSet();

        var regionIds = await _regionRepository.GetAncestorIdsAsync(supplier.RegionId, cancellationToken);
        regionIds.Add(supplier.RegionId);
        var regionSet = regionIds.ToHashSet();

        var allOrders = await _groupOrderRepository.GetAllAsync(cancellationToken);
        var orders = allOrders.Where(o =>
            assignedIds.Contains(o.Id) ||
            regionSet.Contains(o.RegionId)
        ).ToList();

        if (!string.IsNullOrEmpty(status))
            orders = orders.Where(o => o.Status == status).ToList();

        var total = orders.Count;
        var paged = orders.Skip((page - 1) * limit).Take(limit).ToList();

        var items = paged.Select(o => new SupplierOrderListDto
        {
            Id = o.Id,
            Title = o.Title,
            CreatorName = o.Creator?.User?.FullName ?? "",
            BuyerCompany = o.Creator?.BusinessName,
            TotalAmount = o.Items?.Sum(i => (i.UnitPrice ?? 0) * i.TargetQty) ?? 0,
            Status = o.Status,
            Deadline = o.DeadlineAt,
            Region = o.Region?.NameEn ?? "",
            ReceivedAt = o.CreatedAt,
            Items = o.Items?.Select(i => new SupplierOrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? "",
                Quantity = i.ParticipantItems?.Sum(pi => pi.Quantity) ?? 0,
                UnitPrice = i.UnitPrice ?? 0,
                LineTotal = (i.UnitPrice ?? 0) * (i.ParticipantItems?.Sum(pi => pi.Quantity) ?? 0)
            }).ToList() ?? []
        }).ToList();

        return new PaginatedResult<SupplierOrderListDto>
        {
            Items = items,
            Page = page,
            Limit = limit,
            Total = total,
            TotalPages = (int)Math.Ceiling((double)total / limit)
        };
    }

    public async Task<object> AcceptOrderAsync(Guid orderId, Guid userId, string? notes = null, CancellationToken cancellationToken = default)
    {
        var order = await _groupOrderRepository.GetWithDetailsAsync(orderId, cancellationToken)
            ?? throw new KeyNotFoundException("Order not found.");

        if (order.Status != OrderStatus.Open)
            throw new InvalidOperationException("Order is not open for acceptance.");

        _groupOrderRepository.Update(order);

        _eventRepository.Add(new GroupOrderEvent
        {
            Id = Guid.NewGuid(),
            GroupOrderId = order.Id,
            EventType = "SupplierApproved",
            NotesEn = notes,
            CreatedBy = userId
        });

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Notify all participants (creator + joined buyers)
        var participantUserIds = GetAllParticipantUserIds(order);
        var supplier = await _supplierRepository.GetByUserIdAsync(userId, cancellationToken);
        var supplierName = supplier?.CompanyName ?? "المورد";

        foreach (var participantUserId in participantUserIds)
        {
            _notificationRepository.Add(new Notification
            {
                Id = Guid.NewGuid(),
                UserId = participantUserId,
                Type = "SupplierAcceptedOrder",
                TitleAr = "تم قبول طلبك الجماعي",
                TitleEn = "Your Group Order Was Accepted",
                BodyAr = $"قبل المورد '{supplierName}' طلبك الجماعي '{order.Title}'.",
                BodyEn = $"Supplier '{supplierName}' has accepted the group order '{order.Title}'.",
                Channel = "InApp",
                RelatedOrderId = orderId
            });
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new { message = "Order accepted", orderStatus = OrderStatus.Open };
    }

    public async Task<object> DeclineOrderAsync(Guid orderId, Guid userId, string reason, CancellationToken cancellationToken = default)
    {
        var order = await _groupOrderRepository.GetWithDetailsAsync(orderId, cancellationToken)
            ?? throw new KeyNotFoundException("Order not found.");

        if (order.Status != OrderStatus.Open)
            throw new InvalidOperationException("Order is not open for acceptance.");

        order.Status = OrderStatus.Cancelled;
        _groupOrderRepository.Update(order);

        _eventRepository.Add(new GroupOrderEvent
        {
            Id = Guid.NewGuid(),
            GroupOrderId = order.Id,
            EventType = "SupplierDeclined",
            NotesEn = reason,
            CreatedBy = userId
        });

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Notify all participants (creator + joined buyers)
        var participantUserIds = GetAllParticipantUserIds(order);
        var supplier = await _supplierRepository.GetByUserIdAsync(userId, cancellationToken);
        var supplierName = supplier?.CompanyName ?? "المورد";

        foreach (var participantUserId in participantUserIds)
        {
            _notificationRepository.Add(new Notification
            {
                Id = Guid.NewGuid(),
                UserId = participantUserId,
                Type = "SupplierDeclinedOrder",
                TitleAr = "تم رفض طلبك الجماعي",
                TitleEn = "Your Group Order Was Declined",
                BodyAr = $"رفض المورد '{supplierName}' طلبك الجماعي '{order.Title}'. السبب: {reason}",
                BodyEn = $"Supplier '{supplierName}' has declined the group order '{order.Title}'. Reason: {reason}",
                Channel = "InApp",
                RelatedOrderId = orderId
            });
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new { message = "Order declined", orderStatus = OrderStatus.Cancelled };
    }

    public async Task<PaginatedResult<DeliveryListDto>> GetDeliveriesAsync(Guid userId, string? status = null, int page = 1, int limit = 20, CancellationToken cancellationToken = default)
    {
        var supplier = await _supplierRepository.GetByUserIdAsync(userId, cancellationToken);
        if (supplier == null) return new PaginatedResult<DeliveryListDto> { Items = [], Page = page, Limit = limit, Total = 0, TotalPages = 0 };
        if (!supplier.IsApproved)
            throw new InvalidOperationException("Account pending approval.");

        var allDeliveries = await _deliveryRepository.GetAllAsync(cancellationToken);
        var deliveries = allDeliveries.Where(d => d.SupplierId == supplier.Id).ToList();

        if (!string.IsNullOrEmpty(status))
            deliveries = deliveries.Where(d => d.Status == status).ToList();

        var total = deliveries.Count;
        var paged = deliveries.Skip((page - 1) * limit).Take(limit).ToList();

        var items = paged.Select(d => new DeliveryListDto
        {
            Id = d.Id,
            OrderId = d.GroupOrderId,
            Title = d.GroupOrder?.Title ?? "",
            Address = d.ShippingAddress,
            Status = d.Status,
            ScheduledAt = d.ScheduledAt,
            DeliveredAt = d.DeliveredAt,
            TrackingNotes = d.TrackingNotes,
            BuyerName = d.Invoice?.Buyer?.User?.FullName ?? "",
            Items = (d.GroupOrder?.Items?.Select(i => new DeliveryItemDto
            {
                ProductName = i.Product?.Name ?? "",
                Quantity = i.ParticipantItems?.Sum(pi => pi.Quantity) ?? 0
            }).ToList() ?? [])
        }).ToList();

        return new PaginatedResult<DeliveryListDto>
        {
            Items = items,
            Page = page,
            Limit = limit,
            Total = total,
            TotalPages = (int)Math.Ceiling((double)total / limit)
        };
    }

    public async Task<object> UpdateDeliveryStatusAsync(Guid deliveryId, string status, string? trackingNotes = null, DateTimeOffset? scheduledAt = null, CancellationToken cancellationToken = default)
    {
        var delivery = await _deliveryRepository.GetByIdAsync(deliveryId, cancellationToken)
            ?? throw new KeyNotFoundException("Delivery not found.");

        delivery.Status = status;
        if (trackingNotes != null) delivery.TrackingNotes = trackingNotes;
        if (scheduledAt.HasValue) delivery.ScheduledAt = scheduledAt.Value;
        if (status == "Delivered") delivery.DeliveredAt = DateTimeOffset.UtcNow;

        _deliveryRepository.Update(delivery);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Notify the buyer linked to this delivery's invoice
        var buyerUserId = delivery.Invoice?.Buyer?.UserId;
        if (buyerUserId.HasValue)
        {
            var (titleAr, titleEn, bodyAr, bodyEn) = status switch
            {
                "Scheduled" => ("تم جدولة التسليم", "Delivery Scheduled",
                    $"تم جدولة تسليم طلبك بتاريخ {scheduledAt?.ToString("yyyy-MM-dd") ?? "قريبًا"}.",
                    $"Your delivery has been scheduled for {scheduledAt?.ToString("yyyy-MM-dd") ?? "soon"}."),
                "OutForDelivery" => ("طلبك في الطريق إليك", "Out for Delivery",
                    "طلبك في طريقه إليك الآن!",
                    "Your order is on its way!"),
                "Delivered" => ("تم تسليم طلبك", "Order Delivered",
                    "تم تسليم طلبك بنجاح. نأمل أن تكون راضيًا!",
                    "Your order has been delivered successfully. We hope you're satisfied!"),
                _ => ("تحديث حالة التوصيل", "Delivery Status Updated",
                    $"تم تحديث حالة توصيل طلبك إلى: {status}.",
                    $"Your delivery status has been updated to: {status}.")
            };

            _notificationRepository.Add(new Notification
            {
                Id = Guid.NewGuid(),
                UserId = buyerUserId.Value,
                Type = "DeliveryStatusUpdated",
                TitleAr = titleAr,
                TitleEn = titleEn,
                BodyAr = bodyAr,
                BodyEn = bodyEn,
                Channel = "InApp",
                RelatedOrderId = delivery.GroupOrderId
            });
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return new { message = "Delivery status updated", delivery };
    }

    private static List<Guid> GetAllParticipantUserIds(GroupOrder order)
    {
        var ids = new List<Guid>();

        // Creator
        if (order.Creator?.UserId is Guid creatorId)
            ids.Add(creatorId);

        // All joined participants (excluding creator to avoid duplicates)
        var creatorBuyerId = order.CreatorId;
        var participantIds = order.Participants?
            .Where(p => p.Status == "Joined" && p.BuyerId != creatorBuyerId && p.Buyer?.UserId != null)
            .Select(p => p.Buyer!.UserId)
            .ToList() ?? [];

        ids.AddRange(participantIds);
        return ids.Distinct().ToList();
    }
}
