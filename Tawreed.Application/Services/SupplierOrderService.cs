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
            NotesAr = $"ÙˆØ§ÙÙ‚ Ø§Ù„Ù…ÙˆØ±Ø¯ Ø¹Ù„Ù‰ Ø§Ù„Ø¹Ù†Ø§ØµØ±: {string.Join("ØŒ ", acceptedProductNames)}",
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

        // Create invoice for the creator for any unclaimed portion of this supplier's items
        var creatorShareItems = myPendingItems
            .Select(i => new
            {
                Item = i,
                ClaimedQty = order.Participants?
                    .Where(p => p.Status == "Joined")
                    .SelectMany(p => p.Items ?? Enumerable.Empty<ParticipantItem>())
                    .Where(pi => pi.GroupOrderItemId == i.Id)
                    .Sum(pi => pi.Quantity) ?? 0
            })
            .Where(x => x.ClaimedQty < x.Item.TargetQty)
            .ToList();

        if (creatorShareItems.Count > 0 && order.Creator?.UserId is Guid creatorUserId)
        {
            decimal creatorSubtotal = creatorShareItems
                .Sum(x => (x.Item.UnitPrice ?? 0) * (x.Item.TargetQty - x.ClaimedQty));

            _invoiceRepository.Add(new Invoice
            {
                Id = Guid.NewGuid(),
                InvoiceNumber = $"INV-{DateTimeOffset.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}"[..30],
                GroupOrderId = order.Id,
                BuyerId = order.CreatorId,
                ParticipantId = null,
                Subtotal = creatorSubtotal,
                DeliveryFee = 0,
                Total = creatorSubtotal,
                PaymentMethod = "Cash",
                PaymentStatus = "Unpaid",
                ShippingRegion = shippingRegion,
                VerificationCode = Random.Shared.Next(100000, 999999).ToString()
            });
        }

        // Create delivery for this supplier's items
        var existingDelivery = order.Deliveries?.FirstOrDefault(d => d.SupplierId == supplier.Id);
        if (existingDelivery == null)
        {
            existingDelivery = new Delivery
            {
                Id = Guid.NewGuid(),
                GroupOrderId = order.Id,
                SupplierId = supplier.Id,
                Status = "Pending",
                ScheduledAt = request.ScheduledDeliveryAt,
                TrackingNotes = request.DeliveryNotes,
                ShippingRegion = shippingRegion
            };
            _deliveryRepository.Add(existingDelivery);
        }

        // Auto-assign the best available delivery person for this region
        var regionId = order.Region?.Id ?? order.RegionId;
        var deliveryProfiles = (await _deliveryPersonProfileRepository.FindAsync(
            p => p.IsActive && p.CoverageRegionId == regionId, cancellationToken)).ToList();
        if (deliveryProfiles.Count > 0)
        {
            var chosen = deliveryProfiles
                .OrderBy(p => p.BaseDeliveryFee)
                .ThenByDescending(p => p.Rating)
                .First();
            existingDelivery.DeliveryPersonId = chosen.UserId;
            existingDelivery.DeliveryFee = chosen.BaseDeliveryFee;
            existingDelivery.DeliveryType = "System";

            // Notify the assigned delivery person
            _notificationRepository.Add(new Notification
                {
                    Id = Guid.NewGuid(),
                    UserId = chosen.UserId,
                    Type = "DeliveryAssigned",
                    TitleAr = "تم تعيين توصيل لك",
                    TitleEn = "Delivery Assigned to You",
                    BodyAr = $"تم تعيينك لتوصيل طلب '{order.Title}'. يرجى مراجعة تفاصيل التوصيل.",
                    BodyEn = $"You have been assigned to deliver order '{order.Title}'. Please review delivery details.",
                    Channel = "InApp",
                    RelatedOrderId = order.Id
                });
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
                ? $"Ø±ÙØ¶ Ø§Ù„Ù…ÙˆØ±Ø¯ Ø§Ù„Ø¹Ù†Ø§ØµØ±: {string.Join("ØŒ ", declinedProductNames)}"
                : $"Ø§Ù„Ø³Ø¨Ø¨: {reason} - Ø§Ù„Ø¹Ù†Ø§ØµØ±: {string.Join("ØŒ ", declinedProductNames)}",
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
                         (string.IsNullOrEmpty(reason) ? "" : $" Ø§Ù„Ø³Ø¨Ø¨: {reason}"),
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
                "Scheduled" => ("ØªÙ… Ø¬Ø¯ÙˆÙ„Ø© Ø§Ù„ØªØ³Ù„ÙŠÙ…", "Delivery Scheduled",
                    $"ØªÙ… Ø¬Ø¯ÙˆÙ„Ø© ØªØ³Ù„ÙŠÙ… Ø·Ù„Ø¨Ùƒ Ø¨ØªØ§Ø±ÙŠØ® {scheduledAt?.ToString("yyyy-MM-dd") ?? "Ù‚Ø±ÙŠØ¨Ù‹Ø§"}.",
                    $"Your delivery has been scheduled for {scheduledAt?.ToString("yyyy-MM-dd") ?? "soon"}."),
                "OutForDelivery" => ("Ø·Ù„Ø¨Ùƒ ÙÙŠ Ø§Ù„Ø·Ø±ÙŠÙ‚ Ø¥Ù„ÙŠÙƒ", "Out for Delivery",
                    "Ø·Ù„Ø¨Ùƒ ÙÙŠ Ø·Ø±ÙŠÙ‚Ù‡ Ø¥Ù„ÙŠÙƒ Ø§Ù„Ø¢Ù†!",
                    "Your order is on its way!"),
                "Delivered" => ("ØªÙ… ØªØ³Ù„ÙŠÙ… Ø·Ù„Ø¨Ùƒ", "Order Delivered",
                    "ØªÙ… ØªØ³Ù„ÙŠÙ… Ø·Ù„Ø¨Ùƒ Ø¨Ù†Ø¬Ø§Ø­. Ù†Ø£Ù…Ù„ Ø£Ù† ØªÙƒÙˆÙ† Ø±Ø§Ø¶ÙŠÙ‹Ø§!",
                    "Your order has been delivered successfully. We hope you're satisfied!"),
                _ => ("ØªØ­Ø¯ÙŠØ« Ø­Ø§Ù„Ø© Ø§Ù„ØªÙˆØµÙŠÙ„", "Delivery Status Updated",
                    $"ØªÙ… ØªØ­Ø¯ÙŠØ« Ø­Ø§Ù„Ø© ØªÙˆØµÙŠÙ„ Ø·Ù„Ø¨Ùƒ Ø¥Ù„Ù‰: {status}.",
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
