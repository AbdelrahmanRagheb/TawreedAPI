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
    private readonly IGroupOrderItemRepository _groupOrderItemRepository;
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly IRegionRepository _regionRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IDeliveryPersonProfileRepository _deliveryPersonProfileRepository;
    private readonly IDeliveryAssignmentRequestRepository _deliveryAssignmentRequestRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SupplierOrderService(
        IGroupOrderRepository groupOrderRepository,
        IGroupOrderEventRepository eventRepository,
        IGroupOrderItemRepository groupOrderItemRepository,
        IDeliveryRepository deliveryRepository,
        ISupplierRepository supplierRepository,
        IRegionRepository regionRepository,
        IInvoiceRepository invoiceRepository,
        INotificationRepository notificationRepository,
        IDeliveryPersonProfileRepository deliveryPersonProfileRepository,
        IDeliveryAssignmentRequestRepository deliveryAssignmentRequestRepository,
        IUnitOfWork unitOfWork)
    {
        _groupOrderRepository = groupOrderRepository;
        _eventRepository = eventRepository;
        _groupOrderItemRepository = groupOrderItemRepository;
        _deliveryRepository = deliveryRepository;
        _supplierRepository = supplierRepository;
        _regionRepository = regionRepository;
        _invoiceRepository = invoiceRepository;
        _notificationRepository = notificationRepository;
        _deliveryPersonProfileRepository = deliveryPersonProfileRepository;
        _deliveryAssignmentRequestRepository = deliveryAssignmentRequestRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedResult<SupplierOrderListDto>> GetOrdersAsync(Guid userId, string? status = null, int page = 1, int limit = 20, CancellationToken cancellationToken = default)
    {
        var supplier = await _supplierRepository.GetByUserIdAsync(userId, cancellationToken);
        if (supplier == null) return new PaginatedResult<SupplierOrderListDto> { Items = [], Page = page, Limit = limit, Total = 0, TotalPages = 0 };
        if (!supplier.IsApproved)
            throw new InvalidOperationException("Account pending approval.");

        // Get orders where supplier has items assigned
        var allOrders = await _groupOrderRepository.GetAllAsync(cancellationToken);
        var ordersWithMyItems = allOrders
            .Where(o => o.Items != null && o.Items.Any(i => i.SupplierId == supplier.Id))
            .ToList();

        // Also include orders in the supplier's region that have unassigned items
        var regionIds = await _regionRepository.GetAncestorIdsAsync(supplier.RegionId, cancellationToken);
        regionIds.Add(supplier.RegionId);
        var regionSet = regionIds.ToHashSet();

        var regionOrders = allOrders
            .Where(o => regionSet.Contains(o.RegionId) && o.Items != null && o.Items.Any(i => i.ItemStatus == "Unassigned"))
            .ToList();

        var orders = ordersWithMyItems
            .UnionBy(regionOrders, o => o.Id)
            .ToList();

        if (!string.IsNullOrEmpty(status))
            orders = orders.Where(o => o.Status == status).ToList();

        var total = orders.Count;
        var paged = orders.Skip((page - 1) * limit).Take(limit).ToList();

        var items = paged.Select(o =>
        {
            // Only show items assigned to this supplier (or unassigned items in region)
            var myItems = o.Items?
                .Where(i => i.SupplierId == supplier.Id || (i.ItemStatus == "Unassigned" && regionSet.Contains(o.RegionId)))
                .Select(i => new SupplierOrderItemDto
                {
                    GroupOrderItemId = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? "",
                    Quantity = i.SupplierId == supplier.Id ? i.TargetQty : 0,
                    UnitPrice = i.UnitPrice ?? 0,
                    LineTotal = (i.UnitPrice ?? 0) * (i.SupplierId == supplier.Id ? i.TargetQty : 0),
                    ItemStatus = i.ItemStatus
                }).ToList() ?? [];

            return new SupplierOrderListDto
            {
                Id = o.Id,
                Title = o.Title,
                CreatorName = o.Creator?.User?.FullName ?? "",
                BuyerCompany = o.Creator?.BusinessName,
                TotalAmount = myItems.Sum(i => i.LineTotal),
                Status = o.Status,
                Deadline = o.DeadlineAt,
                Region = o.Region?.NameEn ?? "",
                ReceivedAt = o.CreatedAt,
                Items = myItems
            };
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

    public async Task<object> AcceptOrderAsync(Guid orderId, Guid userId, AcceptOrderRequest request, CancellationToken cancellationToken = default)
    {
        var order = await _groupOrderRepository.GetWithDetailsAsync(orderId, cancellationToken)
            ?? throw new KeyNotFoundException("Order not found.");

        var supplier = await _supplierRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("Supplier profile not found.");

        if (order.Items == null || !order.Items.Any(i => i.SupplierId == supplier.Id))
            throw new InvalidOperationException("This order has no items assigned to you.");

        // Accept all pending items assigned to this supplier
        var myPendingItems = order.Items.Where(i => i.SupplierId == supplier.Id && i.ItemStatus == "Pending").ToList();
        if (myPendingItems.Count == 0)
            throw new InvalidOperationException("No pending items to accept.");

        foreach (var item in myPendingItems)
        {
            item.ItemStatus = "Accepted";
            _groupOrderItemRepository.Update(item);
        }

        var acceptedProductNames = myPendingItems.Select(i => i.Product?.Name ?? "Unknown").ToList();

        // Check if all items in the order are now Accepted or Declined
        var allItemsResolved = order.Items.All(i => i.ItemStatus == "Accepted" || i.ItemStatus == "Declined");
        if (allItemsResolved)
        {
            order.Status = OrderStatus.Locked;
            _groupOrderRepository.Update(order);
        }

        _eventRepository.Add(new GroupOrderEvent
        {
            Id = Guid.NewGuid(),
            GroupOrderId = order.Id,
            EventType = "SupplierApproved",
            NotesEn = string.IsNullOrEmpty(request.Notes)
                ? $"Supplier accepted items: {string.Join(", ", acceptedProductNames)}"
                : request.Notes,
            NotesAr = $"وافق المورد على العناصر: {string.Join("، ", acceptedProductNames)}",
            CreatedBy = userId
        });

        // Create invoices for participants (only for this supplier's items)
        var shippingRegion = order.Region?.NameEn ?? "Main Address";
        var joinedParticipants = order.Participants?
            .Where(p => p.Status == "Joined" && p.Items != null && p.Items.Any(i => i.Quantity > 0))
            .ToList() ?? [];

        foreach (var participant in joinedParticipants)
        {
            // Only include items from this supplier
            var supplierItemIds = myPendingItems.Select(i => i.Id).ToHashSet();
            var participantSupplierItems = participant.Items?
                .Where(pi => supplierItemIds.Contains(pi.GroupOrderItemId))
                .ToList() ?? [];

            if (participantSupplierItems.Count == 0) continue;

            decimal subtotal = participantSupplierItems.Sum(pi => pi.Quantity * (pi.GroupOrderItem?.UnitPrice ?? 0));

            var invoice = new Invoice
            {
                Id = Guid.NewGuid(),
                InvoiceNumber = $"INV-{DateTimeOffset.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}"[..30],
                GroupOrderId = order.Id,
                BuyerId = participant.BuyerId,
                ParticipantId = participant.Id,
                Subtotal = subtotal,
                DeliveryFee = 0,
                Total = subtotal,
                PaymentMethod = "Cash",
                PaymentStatus = "Unpaid",
                ShippingRegion = shippingRegion,
                VerificationCode = Random.Shared.Next(100000, 999999).ToString()
            };
            _invoiceRepository.Add(invoice);
        }

        // Create delivery for this supplier's items
        var existingDelivery = order.Deliveries?.FirstOrDefault(d => d.SupplierId == supplier.Id);
        if (existingDelivery == null)
        {
            var delivery = new Delivery
            {
                Id = Guid.NewGuid(),
                GroupOrderId = order.Id,
                SupplierId = supplier.Id,
                Status = "Pending",
                ScheduledAt = request.ScheduledDeliveryAt,
                TrackingNotes = request.DeliveryNotes,
                ShippingRegion = shippingRegion
            };
            _deliveryRepository.Add(delivery);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Notify creator + participants
        var participantUserIds = GetAllParticipantUserIds(order);
        var supplierName = supplier.CompanyName;

        var enMsg = allItemsResolved
            ? $"Supplier '{supplierName}' accepted the final items for order '{order.Title}'."
            : $"Supplier '{supplierName}' accepted items: {string.Join(", ", acceptedProductNames)} for order '{order.Title}'.";

        foreach (var pid in participantUserIds)
        {
            _notificationRepository.Add(new Notification
            {
                Id = Guid.NewGuid(),
                UserId = pid,
                Type = "SupplierAcceptedOrder",
                TitleAr = allItemsResolved ? "تم قبول طلبك الجماعي" : "تم قبول بعض العناصر",
                TitleEn = allItemsResolved ? "Your Group Order Was Accepted" : "Some Items Accepted",
                BodyAr = $"قبل المورد '{supplierName}' العناصر: {string.Join("، ", acceptedProductNames)} في طلب '{order.Title}'.",
                BodyEn = enMsg,
                Channel = "InApp",
                RelatedOrderId = orderId
            });
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new
        {
            message = allItemsResolved ? "All items accepted. Order is locked." : "Supplier items accepted.",
            orderStatus = allItemsResolved ? OrderStatus.Locked : order.Status,
            acceptedItems = acceptedProductNames
        };
    }

    public async Task<object> DeclineOrderAsync(Guid orderId, Guid userId, string reason, CancellationToken cancellationToken = default)
    {
        var order = await _groupOrderRepository.GetWithDetailsAsync(orderId, cancellationToken)
            ?? throw new KeyNotFoundException("Order not found.");

        var supplier = await _supplierRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("Supplier profile not found.");

        if (order.Items == null || !order.Items.Any(i => i.SupplierId == supplier.Id && i.ItemStatus == "Pending"))
            throw new InvalidOperationException("No pending items to decline.");

        // Decline all pending items assigned to this supplier (reset to Unassigned)
        var myPendingItems = order.Items.Where(i => i.SupplierId == supplier.Id && i.ItemStatus == "Pending").ToList();
        foreach (var item in myPendingItems)
        {
            item.SupplierId = null;
            item.SupplierProductId = null;
            item.UnitPrice = null;
            item.ItemStatus = "Unassigned";
            _groupOrderItemRepository.Update(item);
        }

        var declinedProductNames = myPendingItems.Select(i => i.Product?.Name ?? "Unknown").ToList();

        _eventRepository.Add(new GroupOrderEvent
        {
            Id = Guid.NewGuid(),
            GroupOrderId = order.Id,
            EventType = "SupplierDeclined",
            NotesEn = string.IsNullOrEmpty(reason)
                ? $"Supplier declined items: {string.Join(", ", declinedProductNames)}"
                : $"{reason} - Items: {string.Join(", ", declinedProductNames)}",
            NotesAr = string.IsNullOrEmpty(reason)
                ? $"رفض المورد العناصر: {string.Join("، ", declinedProductNames)}"
                : $"السبب: {reason} - العناصر: {string.Join("، ", declinedProductNames)}",
            CreatedBy = userId
        });

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Notify the creator about declined items
        if (order.Creator?.UserId is Guid creatorUserId)
        {
            var supplierName = supplier.CompanyName;
            _notificationRepository.Add(new Notification
            {
                Id = Guid.NewGuid(),
                UserId = creatorUserId,
                Type = "SupplierDeclinedOrder",
                TitleAr = "رفض المورد بعض العناصر",
                TitleEn = "Supplier Declined Items",
                BodyAr = $"رفض المورد '{supplierName}' العناصر: {string.Join("، ", declinedProductNames)} في طلب '{order.Title}'." +
                         (string.IsNullOrEmpty(reason) ? "" : $" السبب: {reason}"),
                BodyEn = $"Supplier '{supplierName}' declined items: {string.Join(", ", declinedProductNames)} in order '{order.Title}'." +
                         (string.IsNullOrEmpty(reason) ? "" : $" Reason: {reason}"),
                Channel = "InApp",
                RelatedOrderId = orderId
            });
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new { message = "Items declined. They are now available for other suppliers.", declinedItems = declinedProductNames };
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
            Address = d.ShippingRegion,
            Status = d.Status,
            ScheduledAt = d.ScheduledAt,
            DeliveredAt = d.DeliveredAt,
            TrackingNotes = d.TrackingNotes,
            BuyerName = d.GroupOrder?.Creator?.User?.FullName ?? "",
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

        // Notify all participants via the group order
        var order = await _groupOrderRepository.GetWithDetailsAsync(delivery.GroupOrderId, cancellationToken);
        if (order != null)
        {
            var participantUserIds = GetAllParticipantUserIds(order);
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

            foreach (var participantUserId in participantUserIds)
            {
                _notificationRepository.Add(new Notification
                {
                    Id = Guid.NewGuid(),
                    UserId = participantUserId,
                    Type = "DeliveryStatusUpdated",
                    TitleAr = titleAr,
                    TitleEn = titleEn,
                    BodyAr = bodyAr,
                    BodyEn = bodyEn,
                    Channel = "InApp",
                    RelatedOrderId = delivery.GroupOrderId
                });
            }
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

    // Cancel/withdraw supplier's items from order
    public async Task<object> CancelOrderFromSupplierAsync(Guid orderId, Guid userId, CancellationToken cancellationToken = default)
    {
        var order = await _groupOrderRepository.GetWithDetailsAsync(orderId, cancellationToken)
            ?? throw new KeyNotFoundException("Order not found.");

        var supplier = await _supplierRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("Supplier profile not found.");

        if (order.Items == null || !order.Items.Any(i => i.SupplierId == supplier.Id))
            throw new InvalidOperationException("This order has no items from you.");

        // Reset all this supplier's items to Unassigned
        var supplierItems = order.Items.Where(i => i.SupplierId == supplier.Id).ToList();
        foreach (var item in supplierItems)
        {
            item.SupplierId = null;
            item.SupplierProductId = null;
            item.UnitPrice = null;
            item.ItemStatus = "Unassigned";
            _groupOrderItemRepository.Update(item);
        }

        // Remove delivery for this supplier
        var delivery = order.Deliveries?.FirstOrDefault(d => d.SupplierId == supplier.Id);
        if (delivery != null)
        {
            _deliveryRepository.Delete(delivery);
        }

        _eventRepository.Add(new GroupOrderEvent
        {
            Id = Guid.NewGuid(),
            GroupOrderId = order.Id,
            EventType = "SupplierCancelled",
            NotesEn = $"Supplier withdrew from items: {string.Join(", ", supplierItems.Select(i => i.Product?.Name ?? "Unknown"))}",
            NotesAr = "ألغى المورد مشاركته في بعض العناصر",
            CreatedBy = userId
        });

        // Notify creator
        if (order.Creator?.UserId is Guid creatorUserId)
        {
            _notificationRepository.Add(new Notification
            {
                Id = Guid.NewGuid(),
                UserId = creatorUserId,
                Type = "SupplierCancelledOrder",
                TitleAr = "تم إلغاء عناصر المورد",
                TitleEn = "Supplier Items Cancelled",
                BodyAr = $"قام المورد '{supplier.CompanyName}' بإلغاء العناصر المخصصة له في الطلب '{order.Title ?? ""}'.",
                BodyEn = $"Supplier '{supplier.CompanyName}' cancelled their items in order '{order.Title ?? ""}'.",
                Channel = "InApp",
                RelatedOrderId = orderId
            });
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new { message = "Supplier items cancelled. They are now unassigned.", cancelledItems = supplierItems.Count };
    }

}
