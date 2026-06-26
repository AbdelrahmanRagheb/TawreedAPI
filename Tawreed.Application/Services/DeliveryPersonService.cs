using System.Text.Json;
using Tawreed.Application.Interfaces;
using Tawreed.Domain.Entities;
using Tawreed.Domain.Enums;
using Tawreed.Domain.Interfaces;

namespace Tawreed.Application.Services;

public class DeliveryPersonService : IDeliveryPersonService
{
    private readonly IDeliveryPersonProfileRepository _profileRepository;
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IGroupOrderRepository _groupOrderRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAppSettingRepository _appSettingRepository;
    private readonly IRegionRepository _regionRepository;
    private readonly IDeliveryAssignmentRequestRepository _deliveryAssignmentRequestRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeliveryPersonService(
        IDeliveryPersonProfileRepository profileRepository,
        IDeliveryRepository deliveryRepository,
        IInvoiceRepository invoiceRepository,
        IGroupOrderRepository groupOrderRepository,
        IUserRepository userRepository,
        IAppSettingRepository appSettingRepository,
        IRegionRepository regionRepository,
        IDeliveryAssignmentRequestRepository deliveryAssignmentRequestRepository,
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _profileRepository = profileRepository;
        _deliveryRepository = deliveryRepository;
        _invoiceRepository = invoiceRepository;
        _groupOrderRepository = groupOrderRepository;
        _userRepository = userRepository;
        _appSettingRepository = appSettingRepository;
        _regionRepository = regionRepository;
        _deliveryAssignmentRequestRepository = deliveryAssignmentRequestRepository;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DeliveryPersonDashboardDto> GetDashboardAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var today = DateTimeOffset.UtcNow.Date;

        var allDeliveries = await _deliveryRepository.GetByDeliveryPersonAsync(userId, cancellationToken);

        var activeDeliveries = allDeliveries.Count(d =>
            d.Status != "Delivered" && d.Status != "Cancelled");

        var completedToday = allDeliveries.Count(d =>
            d.Status == "Delivered" && d.DeliveredAt.HasValue && d.DeliveredAt.Value.Date == today);

        var earningsToday = allDeliveries
            .Where(d => d.Status == "Delivered" && d.DeliveredAt.HasValue && d.DeliveredAt.Value.Date == today)
            .Sum(d => d.DeliveryFee ?? 0);

        var profile = await _profileRepository.GetByUserIdAsync(userId, cancellationToken);

        return new DeliveryPersonDashboardDto
        {
            ActiveDeliveries = activeDeliveries,
            CompletedToday = completedToday,
            Rating = profile?.Rating ?? 0,
            EarningsToday = earningsToday
        };
    }

    public async Task<PaginatedResult<DeliveryPersonDeliveryDto>> GetMyDeliveriesAsync(Guid userId, string? status, int page, int limit, CancellationToken cancellationToken = default)
    {
        var deliveries = await _deliveryRepository.GetByDeliveryPersonAsync(userId, cancellationToken);

        if (!string.IsNullOrEmpty(status))
            deliveries = deliveries.Where(d => d.Status == status).ToList();

        var total = deliveries.Count;
        var paged = deliveries.Skip((page - 1) * limit).Take(limit).ToList();

        var items = paged.Select(d =>
        {
            var orderInvoices = d.GroupOrder?.Invoices?.ToList() ?? [];
            return new DeliveryPersonDeliveryDto
            {
                Id = d.Id,
                OrderId = d.GroupOrderId,
                OrderTitle = d.GroupOrder?.Title ?? "",
                ShippingRegion = d.ShippingRegion,
                Status = d.Status,
                ScheduledAt = d.ScheduledAt?.DateTime,
                Participants = orderInvoices
                    .Where(i => i.Participant == null || i.Participant.Status == "Joined")
                    .Select(i => new DeliveryPersonDeliveryParticipantDto
                    {
                        InvoiceId = i.Id,
                        ParticipantId = i.ParticipantId ?? Guid.Empty,
                        ParticipantName = i.Participant?.Buyer?.User?.FullName ?? i.Buyer?.User?.FullName ?? "",
                        Status = i.Participant?.Status ?? "Joined"
                    }).ToList()
            };
        }).ToList();

        return new PaginatedResult<DeliveryPersonDeliveryDto>
        {
            Items = items,
            Page = page,
            Limit = limit,
            Total = total,
            TotalPages = (int)Math.Ceiling((double)total / limit)
        };
    }

    public async Task<DeliveryPersonDeliveryDetailDto> GetDeliveryDetailAsync(Guid deliveryId, Guid userId, CancellationToken cancellationToken = default)
    {
        var delivery = await _deliveryRepository.GetByIdWithGroupOrderAsync(deliveryId, cancellationToken)
            ?? throw new KeyNotFoundException("Delivery not found.");

        if (delivery.DeliveryPersonId != userId)
            throw new UnauthorizedAccessException("Not authorized to view this delivery.");

        var invoices = await _invoiceRepository.GetByGroupOrderAsync(delivery.GroupOrderId, cancellationToken);

        var joinedInvoices = invoices
            .Where(i => i.Participant == null || i.Participant.Status == "Joined")
            .ToList();

        var groupOrderItems = delivery.GroupOrder?.Items?.ToList() ?? [];

        var participantDetails = joinedInvoices.Select(i =>
        {
            var participantItems = i.Participant?.Items?.ToList() ?? [];
            return new DeliveryPersonDeliveryParticipantDetailDto
            {
                InvoiceId = i.Id,
                ParticipantId = i.ParticipantId ?? Guid.Empty,
                ParticipantName = i.Buyer?.User?.FullName ?? "",
                Email = i.Buyer?.User?.Email ?? "",
                Phone = i.Buyer?.User?.Phone ?? "",
                Address = i.Buyer?.Address ?? i.Buyer?.Region?.NameEn ?? i.ShippingRegion ?? "",
                Status = i.Participant?.Status ?? "Joined",
                VerificationCode = i.VerificationCode ?? "",
                Items = groupOrderItems.Select(goi =>
                {
                    var pi = participantItems.FirstOrDefault(p => p.GroupOrderItemId == goi.Id);
                    var qty = pi?.Quantity ?? 0;
                    var unitPrice = pi?.UnitPriceAtJoin ?? goi.UnitPrice ?? 0;
                    return new DeliveryPersonDeliveryItemDto
                    {
                        Id = goi.Id,
                        Name = goi.Product?.Name ?? "",
                        Quantity = qty,
                        Price = unitPrice,
                        UnitPrice = unitPrice,
                        TotalPrice = qty * unitPrice
                    };
                }).ToList()
            };
        }).ToList();

        // Include the order creator in the delivery list
        var creator = delivery.GroupOrder?.Creator;
        if (creator != null)
        {
            var allParticipantItems = delivery.GroupOrder?.Participants
                ?.SelectMany(p => p.Items ?? Enumerable.Empty<ParticipantItem>())
                .ToList() ?? [];

            var creatorItems = groupOrderItems
                .Select(goi =>
                {
                    var takenQty = allParticipantItems
                        .Where(pi => pi.GroupOrderItemId == goi.Id)
                        .Sum(pi => pi.Quantity);
                    var creatorQty = goi.TargetQty - takenQty;
                    var unitPrice = goi.UnitPrice ?? 0;
                    return new DeliveryPersonDeliveryItemDto
                    {
                        Id = goi.Id,
                        Name = goi.Product?.Name ?? "",
                        Quantity = Math.Max(0, creatorQty),
                        Price = unitPrice,
                        UnitPrice = unitPrice,
                        TotalPrice = Math.Max(0, creatorQty) * unitPrice
                    };
                })
                .ToList();

            participantDetails.Add(new DeliveryPersonDeliveryParticipantDetailDto
            {
                InvoiceId = Guid.Empty,
                ParticipantId = Guid.Empty,
                ParticipantName = creator.User?.FullName ?? "",
                Email = creator.User?.Email ?? "",
                Phone = creator.User?.Phone ?? "",
                    Address = creator.Address ?? creator.Region?.NameEn ?? delivery.ShippingRegion ?? "",
                Status = "Joined",
                VerificationCode = "",
                Items = creatorItems.Where(i => i.Quantity > 0).ToList()
            });
        }

        return new DeliveryPersonDeliveryDetailDto
        {
            Id = delivery.Id,
            OrderId = delivery.GroupOrderId,
            OrderTitle = delivery.GroupOrder?.Title ?? "",
            ShippingRegion = delivery.ShippingRegion,
            Status = delivery.Status,
            ScheduledAt = delivery.ScheduledAt?.DateTime,
            Participants = participantDetails.Select(p => new DeliveryPersonDeliveryParticipantDto
            {
                InvoiceId = p.InvoiceId,
                ParticipantId = p.ParticipantId,
                ParticipantName = p.ParticipantName,
                Status = p.Status
            }).ToList(),
            ParticipantDetails = participantDetails
        };
    }

    public async Task<object> UpdateDeliveryStatusAsync(Guid userId, Guid deliveryId, string status, string? trackingNotes = null, CancellationToken cancellationToken = default)
    {
        var delivery = await _deliveryRepository.GetByIdAsync(deliveryId, cancellationToken)
            ?? throw new KeyNotFoundException("Delivery not found.");

        if (delivery.DeliveryPersonId != userId)
            throw new UnauthorizedAccessException("Not authorized to update this delivery.");

        delivery.Status = status;
        if (trackingNotes != null)
            delivery.TrackingNotes = trackingNotes;
        if (status == "Delivered")
            delivery.DeliveredAt = DateTimeOffset.UtcNow;

        _deliveryRepository.Update(delivery);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new { message = "Delivery status updated" };
    }

    public async Task<object> VerifyDeliveryAsync(Guid userId, Guid invoiceId, string verificationCode, CancellationToken cancellationToken = default)
    {
        var invoice = await _invoiceRepository.GetByIdWithGroupOrderAsync(invoiceId, cancellationToken)
            ?? throw new KeyNotFoundException("Invoice not found.");

        var delivery = invoice.GroupOrder?.Deliveries?.FirstOrDefault(d => d.DeliveryPersonId == userId)
            ?? throw new UnauthorizedAccessException("Not authorized to verify this delivery.");

        if (invoice.VerificationCode != verificationCode)
            throw new InvalidOperationException("Invalid verification code.");

        invoice.PaymentStatus = "Delivered";
        _invoiceRepository.Update(invoice);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new { message = "Delivery verified successfully" };
    }

    public async Task<DeliveryPersonProfileDto> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var profile = await _profileRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("Delivery person profile not found.");

        return new DeliveryPersonProfileDto
        {
            UserId = profile.UserId,
            FullName = profile.User?.FullName ?? "",
            Email = profile.User?.Email ?? "",
            Phone = profile.User?.Phone,
            VehicleType = profile.VehicleType,
            BaseDeliveryFee = profile.BaseDeliveryFee,
            Rating = profile.Rating,
            TotalDeliveries = profile.TotalDeliveries,
            IsActive = profile.IsActive,
            CoverageRegionId = profile.CoverageRegionId,
            CoverageRegionName = profile.CoverageRegion?.NameEn ?? ""
        };
    }

    public async Task UpdateProfileAsync(Guid userId, UpdateDeliveryPersonProfileRequest request, CancellationToken cancellationToken = default)
    {
        var profile = await _profileRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("Delivery person profile not found.");

        if (request.VehicleType != null)
            profile.VehicleType = request.VehicleType;
        if (request.BaseDeliveryFee.HasValue)
            profile.BaseDeliveryFee = request.BaseDeliveryFee.Value;
        if (request.CoverageRegionId.HasValue)
            profile.CoverageRegionId = request.CoverageRegionId;

        _profileRepository.Update(profile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AvailableRegionDto>> GetAvailableRegionsAsync(CancellationToken cancellationToken = default)
    {
        var setting = await _appSettingRepository.GetByKeyAsync("GroupRegionTypes", cancellationToken);
        if (setting is null || string.IsNullOrWhiteSpace(setting.Value))
            return [];

        List<RegionType> allowedTypes;
        try
        {
            var typeNames = JsonSerializer.Deserialize<List<string>>(setting.Value) ?? [];
            allowedTypes = typeNames
                .Select(n => Enum.TryParse<RegionType>(n, true, out var t) ? t : (RegionType?)null)
                .Where(t => t.HasValue)
                .Select(t => t!.Value)
                .ToList();
        }
        catch
        {
            return [];
        }

        if (allowedTypes.Count == 0) return [];

        var allRegions = await _regionRepository.GetAllAsync(cancellationToken);
        return allRegions
            .Where(r => r.IsActive && allowedTypes.Contains(r.Type))
            .Select(r => new AvailableRegionDto
            {
                Id = r.Id,
                NameAr = r.NameAr,
                NameEn = r.NameEn
            })
            .ToList();
    }

    public async Task<IReadOnlyList<PendingDeliveryRequestDto>> GetPendingRequestsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var profile = await _profileRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("Delivery person profile not found.");

        var requests = await _deliveryAssignmentRequestRepository.GetPendingByPersonAsync(profile.Id, cancellationToken);

        return requests.Select(r => new PendingDeliveryRequestDto
        {
            Id = r.Id,
            OrderId = r.OrderId,
            OrderTitle = r.Order?.Title ?? "",
            CreatorName = r.Order?.Creator?.User?.FullName ?? "",
            SupplierName = r.Supplier?.CompanyName ?? "",
            Region = r.Order?.Region?.NameEn ?? "",
            ProposedFee = r.ProposedFee,
            Status = r.Status,
            CreatedAt = r.CreatedAt,
            RespondedAt = r.RespondedAt
        }).ToList();
    }

    public async Task<object> AcceptDeliveryRequestAsync(Guid requestId, Guid userId, CancellationToken cancellationToken = default)
    {
        var profile = await _profileRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("Delivery person profile not found.");

        var request = await _deliveryAssignmentRequestRepository.GetByIdAsync(requestId, cancellationToken)
            ?? throw new KeyNotFoundException("Request not found.");

        if (request.DeliveryPersonId != profile.Id)
            throw new InvalidOperationException("This request is not for you.");

        if (request.Status != "Pending")
            throw new InvalidOperationException("This request is no longer pending.");

        // Check if another person already accepted
        var order = await _groupOrderRepository.GetWithDetailsAsync(request.OrderId, cancellationToken)
            ?? throw new KeyNotFoundException("Order not found.");

        if (order.AssignedDeliveryPersonId.HasValue)
            throw new InvalidOperationException("Another delivery person has already been assigned to this order.");

        // Update request
        request.Status = "Accepted";
        request.RespondedAt = DateTimeOffset.UtcNow;
        _deliveryAssignmentRequestRepository.Update(request);

        // Assign to order
        order.AssignedDeliveryPersonId = profile.Id;
        order.ProposedDeliveryFee = request.ProposedFee ?? profile.BaseDeliveryFee;
        order.DeliveryApprovalStatus = "Pending";
        _groupOrderRepository.Update(order);

        // Update delivery record
        var delivery = await _deliveryRepository.FindOneAsync(d => d.GroupOrderId == request.OrderId, cancellationToken);
        if (delivery != null)
        {
            delivery.DeliveryPersonId = profile.UserId;
            delivery.DeliveryType = "System";
            delivery.DeliveryFee = request.ProposedFee ?? profile.BaseDeliveryFee;
            _deliveryRepository.Update(delivery);
        }

        // Notify supplier
        _notificationRepository.Add(new Notification
        {
            Id = Guid.NewGuid(),
            UserId = order.Supplier?.UserId ?? Guid.Empty,
            Type = "DeliveryRequestAccepted",
            TitleAr = "قبول طلب التوصيل",
            TitleEn = "Delivery Request Accepted",
            BodyAr = $"قام مندوب التوصيل بقبول طلب التوصيل للطلب '{order.Title}'",
            BodyEn = $"The delivery person accepted the delivery request for order '{order.Title}'",
            Channel = "InApp",
            RelatedOrderId = request.OrderId
        });

        // Notify creator for approval
        if (order.Creator?.UserId is Guid creatorUserId)
        {
            _notificationRepository.Add(new Notification
            {
                Id = Guid.NewGuid(),
                UserId = creatorUserId,
                Type = "DeliveryRequestAccepted",
                TitleAr = "تم تعيين مندوب توصيل",
                TitleEn = "Delivery Person Assigned",
                BodyAr = $"تم تعيين مندوب توصيل للطلب '{order.Title}'. رسوم التوصيل: {order.ProposedDeliveryFee}",
                BodyEn = $"A delivery person has been assigned for order '{order.Title}'. Delivery fee: {order.ProposedDeliveryFee}",
                Channel = "InApp",
                RelatedOrderId = request.OrderId
            });
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new { message = "Delivery request accepted.", orderId = request.OrderId };
    }

    public async Task<object> DeclineDeliveryRequestAsync(Guid requestId, Guid userId, string? reason, CancellationToken cancellationToken = default)
    {
        var profile = await _profileRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("Delivery person profile not found.");

        var request = await _deliveryAssignmentRequestRepository.GetByIdAsync(requestId, cancellationToken)
            ?? throw new KeyNotFoundException("Request not found.");

        if (request.DeliveryPersonId != profile.Id)
            throw new InvalidOperationException("This request is not for you.");

        if (request.Status != "Pending")
            throw new InvalidOperationException("This request is no longer pending.");

        request.Status = "Declined";
        request.RespondedAt = DateTimeOffset.UtcNow;
        request.DeclineReason = reason;
        _deliveryAssignmentRequestRepository.Update(request);

        // Notify supplier
        var order = await _groupOrderRepository.GetWithDetailsAsync(request.OrderId, cancellationToken);
        if (order?.Supplier?.UserId is Guid supplierUserId)
        {
            _notificationRepository.Add(new Notification
            {
                Id = Guid.NewGuid(),
                UserId = supplierUserId,
                Type = "DeliveryRequestDeclined",
                TitleAr = "رفض طلب التوصيل",
                TitleEn = "Delivery Request Declined",
                BodyAr = $"قام مندوب التوصيل برفض طلب التوصيل للطلب '{order.Title}'. {(reason != null ? $"السبب: {reason}" : "")}",
                BodyEn = $"The delivery person declined the delivery request for order '{order.Title}'. {(reason != null ? $"Reason: {reason}" : "")}",
                Channel = "InApp",
                RelatedOrderId = request.OrderId
            });
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new { message = "Delivery request declined.", requestId = request.Id };
    }
}