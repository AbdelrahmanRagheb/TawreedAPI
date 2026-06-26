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
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IDeliveryPersonProfileRepository _deliveryPersonProfileRepository;
    private readonly IDeliveryAssignmentRequestRepository _deliveryAssignmentRequestRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SupplierOrderService(
        IGroupOrderRepository groupOrderRepository,
        IGroupOrderEventRepository eventRepository,
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
            DeliveryPreference = o.DeliveryPreference,
            PreferredDeliveryPersonId = o.PreferredDeliveryPersonId,
            PreferredDeliveryPersonName = o.PreferredDeliveryPerson?.User?.FullName,
            Items = o.Items?.Select(i => new SupplierOrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? "",
                Quantity = i.TargetQty,
                UnitPrice = i.UnitPrice ?? 0,
                LineTotal = (i.UnitPrice ?? 0) * i.TargetQty
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

    public async Task<object> AcceptOrderAsync(Guid orderId, Guid userId, AcceptOrderRequest request, CancellationToken cancellationToken = default)
    {
        var order = await _groupOrderRepository.GetWithDetailsAsync(orderId, cancellationToken)
            ?? throw new KeyNotFoundException("Order not found.");

        if (order.Status != OrderStatus.PendingApproval)
            throw new InvalidOperationException("Order is not pending approval.");

        var supplier = await _supplierRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("Supplier profile not found.");

        if (order.SupplierId != supplier.Id)
            throw new InvalidOperationException("This order is not assigned to you.");

        order.Status = OrderStatus.Locked;
        _groupOrderRepository.Update(order);

        _eventRepository.Add(new GroupOrderEvent
        {
            Id = Guid.NewGuid(),
            GroupOrderId = order.Id,
            EventType = "SupplierApproved",
            NotesEn = string.IsNullOrEmpty(request.Notes) ? "The supplier has accepted the order" : request.Notes,
            NotesAr = "وافق المورد على الطلب",
            CreatedBy = userId
        });

        // Create invoices and deliveries for each participant
        var shippingRegion = order.Region?.NameEn ?? "Main Address";
        var joinedParticipants = order.Participants?.Where(p => p.Status == "Joined" && p.Items != null && p.Items.Any(i => i.Quantity > 0)).ToList() ?? [];

        foreach (var participant in joinedParticipants)
        {
            decimal subtotal = participant.Items!.Sum(pi => pi.Quantity * (pi.GroupOrderItem?.UnitPrice ?? 0));

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

        // Create a single delivery for the entire order
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

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Notify all participants (creator + joined buyers)
        var participantUserIds = GetAllParticipantUserIds(order);
        var supplierName = supplier.CompanyName;

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

        return new { message = "Order accepted", orderStatus = OrderStatus.Locked };
    }

    public async Task<object> DeclineOrderAsync(Guid orderId, Guid userId, string reason, CancellationToken cancellationToken = default)
    {
        var order = await _groupOrderRepository.GetWithDetailsAsync(orderId, cancellationToken)
            ?? throw new KeyNotFoundException("Order not found.");

        if (order.Status != OrderStatus.PendingApproval)
            throw new InvalidOperationException("Order is not pending approval.");

        var supplier = await _supplierRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("Supplier profile not found.");

        if (order.SupplierId != supplier.Id)
            throw new InvalidOperationException("This order is not assigned to you.");

        order.Status = OrderStatus.Open;
        order.SupplierId = null;
        _groupOrderRepository.Update(order);

        _eventRepository.Add(new GroupOrderEvent
        {
            Id = Guid.NewGuid(),
            GroupOrderId = order.Id,
            EventType = "SupplierDeclined",
            NotesEn = reason,
            NotesAr = $"رفض المورد الطلب. السبب: {reason}",
            CreatedBy = userId
        });

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Notify the creator that the supplier declined
        if (order.Creator?.UserId is Guid creatorUserId)
        {
            var supplierName = supplier.CompanyName;
            _notificationRepository.Add(new Notification
            {
                Id = Guid.NewGuid(),
                UserId = creatorUserId,
                Type = "SupplierDeclinedOrder",
                TitleAr = "رفض المورد الطلب",
                TitleEn = "Supplier Declined Order",
                BodyAr = $"رفض المورد '{supplierName}' طلبك الجماعي '{order.Title}'. السبب: {reason}",
                BodyEn = $"Supplier '{supplierName}' has declined the group order '{order.Title}'. Reason: {reason}",
                Channel = "InApp",
                RelatedOrderId = orderId
            });
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new { message = "Order declined. You can assign a different supplier.", orderStatus = OrderStatus.Open };
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

    // Browse available delivery persons for supplier to choose from
    public async Task<IReadOnlyList<AvailableDeliveryPersonDto>> BrowseAvailableDeliveryPersonsAsync(Guid orderId, Guid userId, CancellationToken cancellationToken = default)
    {
        var order = await _groupOrderRepository.GetWithDetailsAsync(orderId, cancellationToken)
            ?? throw new KeyNotFoundException("Order not found.");

        var supplier = await _supplierRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("Supplier profile not found.");

        if (order.SupplierId != supplier.Id)
            throw new InvalidOperationException("This order is not assigned to you.");

        // If buyer selected a specific delivery person, only show that one
        if (order.DeliveryPreference == "SpecificPerson" && order.PreferredDeliveryPersonId.HasValue)
        {
            var profile = await _deliveryPersonProfileRepository.GetByIdAsync(order.PreferredDeliveryPersonId.Value, cancellationToken);
            if (profile == null) return [];
            return [MapToDto(profile)];
        }

        var regionId = order.RegionId;
        var profiles = await _deliveryPersonProfileRepository.GetForRegionAsync(regionId, cancellationToken);

        return profiles.Select(MapToDto).ToList();
    }

    // Send a delivery request (not direct assignment)
    public async Task<object> RequestDeliveryPersonAsync(Guid orderId, Guid deliveryPersonId, Guid userId, CancellationToken cancellationToken = default)
    {
        var order = await _groupOrderRepository.GetWithDetailsAsync(orderId, cancellationToken)
            ?? throw new KeyNotFoundException("Order not found.");

        var supplier = await _supplierRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("Supplier profile not found.");

        if (order.SupplierId != supplier.Id)
            throw new InvalidOperationException("This order is not assigned to you.");

        if (order.Status != OrderStatus.Locked)
            throw new InvalidOperationException("Order must be locked to assign a delivery person.");

        var deliveryPerson = await _deliveryPersonProfileRepository.GetByIdAsync(deliveryPersonId, cancellationToken)
            ?? throw new KeyNotFoundException("Delivery person not found.");

        if (!deliveryPerson.IsActive)
            throw new InvalidOperationException("Delivery person is not active.");

        if (order.DeliveryPreference == "OwnDelivery")
            throw new InvalidOperationException("Buyer chose to use their own delivery.");

        if (order.DeliveryPreference == "SpecificPerson" && order.PreferredDeliveryPersonId != deliveryPersonId)
            throw new InvalidOperationException("Buyer specified a different delivery person.");

        // Create the request
        var request = new DeliveryAssignmentRequest
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            DeliveryPersonId = deliveryPersonId,
            SupplierId = supplier.Id,
            Status = "Pending",
            ProposedFee = deliveryPerson.BaseDeliveryFee
        };
        _deliveryAssignmentRequestRepository.Add(request);

        // Notify the delivery person
        _notificationRepository.Add(new Notification
        {
            Id = Guid.NewGuid(),
            UserId = deliveryPerson.UserId,
            Type = "DeliveryAssignmentRequest",
            TitleAr = "طلب توصيل جديد",
            TitleEn = "New Delivery Request",
            BodyAr = $"لديك طلب توصيل جديد للطلب '{order.Title}' من المورد {order.Supplier?.CompanyName ?? ""}",
            BodyEn = $"You have a new delivery request for order '{order.Title}' from supplier {order.Supplier?.CompanyName ?? ""}",
            Channel = "InApp",
            RelatedOrderId = orderId
        });

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new { message = "Delivery request sent to provider.", requestId = request.Id };
    }

    // Assign a delivery person to a locked order
    public async Task<object> AssignDeliveryPersonAsync(Guid orderId, Guid deliveryPersonId, Guid userId, CancellationToken cancellationToken = default)
    {
        var order = await _groupOrderRepository.GetWithDetailsAsync(orderId, cancellationToken)
            ?? throw new KeyNotFoundException("Order not found.");

        var supplier = await _supplierRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("Supplier profile not found.");

        if (order.SupplierId != supplier.Id)
            throw new InvalidOperationException("This order is not assigned to you.");

        if (order.Status != OrderStatus.Locked)
            throw new InvalidOperationException("Order must be locked to assign a delivery person.");

        var deliveryPerson = await _deliveryPersonProfileRepository.GetByIdAsync(deliveryPersonId, cancellationToken)
            ?? throw new KeyNotFoundException("Delivery person not found.");

        if (!deliveryPerson.IsActive)
            throw new InvalidOperationException("Delivery person is not active.");

        // If buyer chose own delivery, only allow own
        if (order.DeliveryPreference == "OwnDelivery")
            throw new InvalidOperationException("Buyer chose to use their own delivery.");

        // If buyer chose specific person, only allow that person
        if (order.DeliveryPreference == "SpecificPerson" && order.PreferredDeliveryPersonId != deliveryPersonId)
            throw new InvalidOperationException("Buyer specified a different delivery person.");

        order.AssignedDeliveryPersonId = deliveryPersonId;
        order.ProposedDeliveryFee = deliveryPerson.BaseDeliveryFee;
        order.DeliveryApprovalStatus = "Pending";
        _groupOrderRepository.Update(order);

        // Update delivery record
        var delivery = await _deliveryRepository.FindOneAsync(d => d.GroupOrderId == orderId, cancellationToken);
        if (delivery != null)
        {
            delivery.DeliveryPersonId = deliveryPerson.UserId;
            delivery.DeliveryType = "System";
            delivery.DeliveryFee = deliveryPerson.BaseDeliveryFee;
            _deliveryRepository.Update(delivery);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new { message = "Delivery person assigned. Waiting for buyer approval.", fee = deliveryPerson.BaseDeliveryFee };
    }

    // Propose a delivery fee and send to buyer for approval
    public async Task<object> ProposeDeliveryFeeAsync(Guid orderId, decimal fee, string? notes, Guid userId, CancellationToken cancellationToken = default)
    {
        var order = await _groupOrderRepository.GetWithDetailsAsync(orderId, cancellationToken)
            ?? throw new KeyNotFoundException("Order not found.");

        var supplier = await _supplierRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("Supplier profile not found.");

        if (order.SupplierId != supplier.Id)
            throw new InvalidOperationException("This order is not assigned to you.");

        if (order.Status != OrderStatus.Locked)
            throw new InvalidOperationException("Order must be locked to propose a delivery fee.");

        if (order.AssignedDeliveryPersonId == null)
            throw new InvalidOperationException("No delivery person assigned.");

        order.ProposedDeliveryFee = fee;
        order.DeliveryApprovalStatus = "Pending";
        _groupOrderRepository.Update(order);

        // Update delivery record fee
        var delivery = await _deliveryRepository.FindOneAsync(d => d.GroupOrderId == orderId, cancellationToken);
        if (delivery != null)
        {
            delivery.DeliveryFee = fee;
            _deliveryRepository.Update(delivery);
        }

        // Notify creator for approval
        if (order.Creator?.UserId is Guid creatorUserId)
        {
            _notificationRepository.Add(new Notification
            {
                Id = Guid.NewGuid(),
                UserId = creatorUserId,
                Type = "DeliveryFeeProposal",
                TitleAr = "عرض جديد لرسوم التوصيل",
                TitleEn = "New Delivery Fee Proposal",
                BodyAr = $"قدم المورد عرض لرسوم التوصيل لطلبك '{order.Title ?? ""}'. يرجى المراجعة. Fee: {fee}",
                BodyEn = $"The supplier proposed a delivery fee for '{order.Title ?? ""}'. Please review and approve. Fee: {fee}",
                Channel = "InApp",
                RelatedOrderId = orderId
            });
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new { message = "Delivery fee proposed. Waiting for buyer approval.", proposedFee = fee };
    }

    // Mark delivery as supplier's own delivery (free for participants)
    public async Task<object> UseOwnDeliveryAsync(Guid orderId, Guid userId, CancellationToken cancellationToken = default)
    {
        var order = await _groupOrderRepository.GetWithDetailsAsync(orderId, cancellationToken)
            ?? throw new KeyNotFoundException("Order not found.");

        var supplier = await _supplierRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("Supplier profile not found.");

        if (order.SupplierId != supplier.Id)
            throw new InvalidOperationException("This order is not assigned to you.");

        if (order.DeliveryPreference == "SpecificPerson" || order.DeliveryPreference == "SystemDelivery")
            throw new InvalidOperationException("Buyer chose to use a system delivery person.");

        order.AssignedDeliveryPersonId = null;
        order.ProposedDeliveryFee = 0;
        order.DeliveryApprovalStatus = null;
        _groupOrderRepository.Update(order);

        var delivery = await _deliveryRepository.FindOneAsync(d => d.GroupOrderId == orderId, cancellationToken);
        if (delivery != null)
        {
            delivery.DeliveryPersonId = null;
            delivery.DeliveryType = "Own";
            delivery.DeliveryFee = 0;
            _deliveryRepository.Update(delivery);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new { message = "Delivery set to own delivery (free)." };
    }

    // Cancel order from supplier side (if buyer rejects delivery fee or for any other reason)
    public async Task<object> CancelOrderFromSupplierAsync(Guid orderId, Guid userId, CancellationToken cancellationToken = default)
    {
        var order = await _groupOrderRepository.GetWithDetailsAsync(orderId, cancellationToken)
            ?? throw new KeyNotFoundException("Order not found.");

        var supplier = await _supplierRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("Supplier profile not found.");

        if (order.SupplierId != supplier.Id)
            throw new InvalidOperationException("This order is not assigned to you.");

        order.Status = OrderStatus.Open;
        order.SupplierId = null;
        order.AssignedDeliveryPersonId = null;
        order.ProposedDeliveryFee = null;
        order.DeliveryApprovalStatus = null;
        order.DeliveryPreference = "None";
        _groupOrderRepository.Update(order);

        _eventRepository.Add(new GroupOrderEvent
        {
            Id = Guid.NewGuid(),
            GroupOrderId = order.Id,
            EventType = "SupplierCancelled",
            NotesEn = "Supplier cancelled the order",
            NotesAr = "ألغى المورد الطلب",
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
                TitleAr = "تم إلغاء الطلب",
                TitleEn = "Order Cancelled",
                BodyAr = $"قام المورد بإلغاء الطلب '{order.Title ?? ""}'.",
                BodyEn = $"The supplier cancelled the order '{order.Title ?? ""}'.",
                Channel = "InApp",
                RelatedOrderId = orderId
            });
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new { message = "Order cancelled. It is now open again.", orderStatus = OrderStatus.Open };
    }

    private static AvailableDeliveryPersonDto MapToDto(DeliveryPersonProfile profile)
    {
return new AvailableDeliveryPersonDto
        {
            Id = profile.Id,
            FullName = profile.User?.FullName ?? "",
            Rating = (double)profile.Rating,
            TotalDeliveries = profile.TotalDeliveries,
            BaseDeliveryFee = profile.BaseDeliveryFee,
            VehicleType = profile.VehicleType,
            CoverageRegionId = profile.CoverageRegionId,
            CoverageRegionName = profile.CoverageRegion?.NameEn
        };
    }
}
