using System.Text.Json;
using Tawreed.Application.Common.Models;
using Tawreed.Application.Interfaces;
using Tawreed.Domain.Entities;
using Tawreed.Domain.Enums;
using Tawreed.Domain.Interfaces;

namespace Tawreed.Application.Services;

public class BuyerOrderService : IBuyerOrderService
{
    private readonly IGroupOrderRepository _groupOrderRepository;
    private readonly IGroupOrderItemRepository _groupOrderItemRepository;
    private readonly IGroupOrderParticipantRepository _participantRepository;
    private readonly IGroupOrderEventRepository _eventRepository;
    private readonly IParticipantItemRepository _participantItemRepository;
    private readonly IBuyerRepository _buyerRepository;
    private readonly ISupplierProductRepository _supplierProductRepository;
    private readonly IRegionRepository _regionRepository;
    private readonly IAppSettingRepository _appSettingRepository;
    private readonly IProductRepository _productRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IDeliveryPersonProfileRepository _deliveryPersonProfileRepository;
    private readonly IDeliveryAssignmentRequestRepository _deliveryAssignmentRequestRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BuyerOrderService(
        IGroupOrderRepository groupOrderRepository,
        IGroupOrderItemRepository groupOrderItemRepository,
        IGroupOrderParticipantRepository participantRepository,
        IGroupOrderEventRepository eventRepository,
        IParticipantItemRepository participantItemRepository,
        IBuyerRepository buyerRepository,
        ISupplierProductRepository supplierProductRepository,
        IRegionRepository regionRepository,
        IAppSettingRepository appSettingRepository,
        IProductRepository productRepository,
        INotificationRepository notificationRepository,
        ISupplierRepository supplierRepository,
        IInvoiceRepository invoiceRepository,
        IDeliveryRepository deliveryRepository,
        IDeliveryPersonProfileRepository deliveryPersonProfileRepository,
        IDeliveryAssignmentRequestRepository deliveryAssignmentRequestRepository,
        IUnitOfWork unitOfWork)
    {
        _groupOrderRepository = groupOrderRepository;
        _groupOrderItemRepository = groupOrderItemRepository;
        _participantRepository = participantRepository;
        _eventRepository = eventRepository;
        _participantItemRepository = participantItemRepository;
        _buyerRepository = buyerRepository;
        _supplierProductRepository = supplierProductRepository;
        _regionRepository = regionRepository;
        _appSettingRepository = appSettingRepository;
        _productRepository = productRepository;
        _notificationRepository = notificationRepository;
        _supplierRepository = supplierRepository;
        _invoiceRepository = invoiceRepository;
        _deliveryRepository = deliveryRepository;
        _deliveryPersonProfileRepository = deliveryPersonProfileRepository;
        _deliveryAssignmentRequestRepository = deliveryAssignmentRequestRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedResult<OrderListDto>> GetOrdersAsync(Guid userId, string? status = null, int page = 1, int limit = 20, CancellationToken cancellationToken = default)
    {
        var buyer = await _buyerRepository.GetByUserIdAsync(userId, cancellationToken);
        if (buyer == null) return new PaginatedResult<OrderListDto> { Items = [], Page = page, Limit = limit, Total = 0, TotalPages = 0 };

        var participations = await _participantRepository.GetByBuyerAsync(buyer.Id, cancellationToken);
        var participatedIds = participations.Select(p => p.GroupOrderId).ToHashSet();

        var regionIds = await _regionRepository.GetAncestorIdsAsync(buyer.RegionId, cancellationToken);
        regionIds.Add(buyer.RegionId);
        var regionSet = regionIds.ToHashSet();

        var allOrders = await _groupOrderRepository.GetAllAsync(cancellationToken);
        var myOrders = allOrders.Where(o =>
            o.CreatorId == buyer.Id ||
            participatedIds.Contains(o.Id) ||
            regionSet.Contains(o.RegionId)
        ).ToList();

        if (!string.IsNullOrEmpty(status))
            myOrders = myOrders.Where(o => o.Status == status).ToList();

        var total = myOrders.Count;
        var paged = myOrders.Skip((page - 1) * limit).Take(limit).ToList();

        var items = paged.Select(o => new OrderListDto
        {
            Id = o.Id,
            Title = o.Title,
            Status = o.Status,
            CreatedAt = o.CreatedAt,
            Deadline = o.DeadlineAt,
            TotalOrderValue = o.Items?.Sum(i => (i.UnitPrice ?? 0) * i.TargetQty) ?? 0,
            ParticipantCount = o.Participants?.Count(p => p.Status == "Joined") ?? 0,
            ProductCount = o.Items?.Count ?? 0,
            Region = o.Region?.NameEn ?? "",
            CreatorName = o.Creator?.User?.FullName ?? "",
            CreatorId = o.CreatorId,
            IsCreator = o.CreatorId == buyer.Id,
            IsParticipant = participatedIds.Contains(o.Id)
        }).ToList();

        return new PaginatedResult<OrderListDto>
        {
            Items = items,
            Page = page,
            Limit = limit,
            Total = total,
            TotalPages = (int)Math.Ceiling((double)total / limit)
        };
    }

    public async Task<OrderDetailDto> GetOrderDetailAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _groupOrderRepository.GetWithDetailsAsync(orderId, cancellationToken)
            ?? throw new KeyNotFoundException("Order not found.");

        var delivery = order.Deliveries?.FirstOrDefault();
        var products = order.Items?.Select(i =>
        {
            var currentQty = i.ParticipantItems?.Sum(pi => pi.Quantity) ?? 0;
            return new OrderProductDto
            {
                GroupOrderItemId = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? "",
                CategoryId = i.Product?.CategoryId ?? Guid.Empty,
                CurrentQuantity = currentQty,
                TargetQuantity = i.TargetQty,
                Unit = i.Product?.Unit?.Symbol ?? "",
                UnitPrice = i.UnitPrice,
                MarketPrice = i.Product?.MarketPrice,
                SupplierProductId = i.SupplierProductId,
                SupplierId = i.SupplierId,
                SupplierName = i.Supplier?.CompanyName ?? "",
                ItemStatus = i.ItemStatus
            };
        }).ToList() ?? [];

        var assignedCount = products.Count(p => p.ItemStatus != "Unassigned");

        var dto = new OrderDetailDto
        {
            Id = order.Id,
            Title = order.Title,
            Description = order.Description,
            CreatorId = order.CreatorId,
            CreatorUserId = order.Creator?.UserId ?? Guid.Empty,
            CreatorName = order.Creator?.User?.FullName ?? "",
            Region = order.Region?.NameEn ?? "",
            CreatedAt = order.CreatedAt,
            Deadline = order.DeadlineAt,
            DeadlinePassed = DateTimeOffset.UtcNow > order.DeadlineAt,
            Status = order.Status,
            TotalOrderValue = products.Sum(p => (p.MarketPrice ?? p.UnitPrice ?? 0) * p.TargetQuantity),
            TotalProductCount = products.Count,
            AssignedProductCount = assignedCount,
            DeliveryPreference = delivery != null ? "SystemDelivery" : null,
            PreferredDeliveryPersonName = delivery?.DeliveryPerson?.FullName,
            ProposedDeliveryFee = delivery?.DeliveryFee,
            DeliveryApprovalStatus = delivery != null ? "Approved" : null,
            AssignedDeliveryPersonName = delivery?.DeliveryPerson?.FullName,
            Products = products,
            Participants = (order.Participants?.Where(p => p.Status == "Joined") ?? Enumerable.Empty<GroupOrderParticipant>()).Select(p => new OrderParticipantDto
            {
                Id = p.Id,
                UserId = p.Buyer?.UserId ?? Guid.Empty,
                Name = p.Buyer?.User?.FullName ?? "",
                JoinedAt = p.JoinedAt,
                Items = p.Items?.Select(pi => new ParticipantItemDto
                {
                    GroupOrderItemId = pi.GroupOrderItemId,
                    SupplierProductId = pi.GroupOrderItem?.SupplierProductId ?? Guid.Empty,
                    ProductName = pi.GroupOrderItem?.Product?.Name ?? "",
                    Quantity = pi.Quantity
                }).ToList() ?? []
            }).ToList(),
            Activities = order.Events?.Select(e => new OrderActivityDto
            {
                Id = e.Id,
                EventType = e.EventType,
                NotesAr = e.NotesAr,
                NotesEn = e.NotesEn,
                CreatedBy = e.Creator?.FullName ?? "",
                CreatedAt = e.CreatedAt
            }).OrderByDescending(a => a.CreatedAt).ToList() ?? []
        };

        return dto;
    }

    public async Task<GroupOrderDto> CreateOrderAsync(Guid userId, CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        var buyer = await _buyerRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("Buyer profile not found.");

        var regionId = await ResolveGroupRegionAsync(buyer.RegionId, cancellationToken);

        var order = new GroupOrder
        {
            Id = Guid.NewGuid(),
            CreatorId = buyer.Id,
            RegionId = regionId,
            Title = request.Title,
            Description = request.Description,
            OrderNumber = $"ORD-{DateTimeOffset.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}"[..20],
            DeadlineAt = request.IsUrgent
                ? DateTimeOffset.UtcNow.AddHours(await GetUrgentDeadlineHoursAsync(cancellationToken))
                : DateTimeOffset.UtcNow.AddDays(await GetDefaultDeadlineDaysAsync(cancellationToken)),
            Status = OrderStatus.Open
        };
        _groupOrderRepository.Add(order);

        foreach (var item in request.Items)
        {
            _groupOrderItemRepository.Add(new GroupOrderItem
            {
                Id = Guid.NewGuid(),
                GroupOrderId = order.Id,
                ProductId = item.ProductId,
                TargetQty = item.TargetQuantity,
                ItemStatus = "Unassigned"
            });
        }

        _eventRepository.Add(new GroupOrderEvent
        {
            Id = Guid.NewGuid(),
            GroupOrderId = order.Id,
            EventType = "Created",
            NotesEn = $"{buyer.BusinessName ?? "A buyer"} created the order",
            NotesAr = $"أنشأ {buyer.BusinessName ?? "المتجر"} طلباً",
            CreatedBy = userId
        });

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Notify all buyers in the same region (excluding the creator)
        var allBuyers = await _buyerRepository.GetAllAsync(cancellationToken);
        var regionBuyers = allBuyers.Where(b => b.RegionId == buyer.RegionId && b.Id != buyer.Id).ToList();
        foreach (var regionBuyer in regionBuyers)
        {
            _notificationRepository.Add(new Notification
            {
                Id = Guid.NewGuid(),
                UserId = regionBuyer.UserId,
                Type = "NewGroupOrder",
                TitleAr = "طلب جماعي جديد في منطقتك",
                TitleEn = "New Group Order in Your Area",
                BodyAr = $"تم إنشاء طلب جماعي جديد '{order.Title}' في منطقتك. انضم الآن!",
                BodyEn = $"A new group order '{order.Title}' has been created in your area. Join now!",
                Channel = "InApp",
                RelatedOrderId = order.Id
            });
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(order);
    }

    public async Task<GroupOrderDto> SaveDraftAsync(Guid userId, CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        var buyer = await _buyerRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("Buyer profile not found.");

        var regionId = await ResolveGroupRegionAsync(buyer.RegionId, cancellationToken);

        var order = new GroupOrder
        {
            Id = Guid.NewGuid(),
            CreatorId = buyer.Id,
            RegionId = regionId,
            Title = request.Title,
            Description = request.Description,
            OrderNumber = $"ORD-{DateTimeOffset.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}"[..20],
            DeadlineAt = request.IsUrgent
                ? DateTimeOffset.UtcNow.AddHours(await GetUrgentDeadlineHoursAsync(cancellationToken))
                : DateTimeOffset.UtcNow.AddDays(await GetDefaultDeadlineDaysAsync(cancellationToken)),
            Status = OrderStatus.Draft
        };
        _groupOrderRepository.Add(order);

        foreach (var item in request.Items)
        {
            _groupOrderItemRepository.Add(new GroupOrderItem
            {
                Id = Guid.NewGuid(),
                GroupOrderId = order.Id,
                ProductId = item.ProductId,
                TargetQty = item.TargetQuantity,
                ItemStatus = "Unassigned"
            });
        }

        _eventRepository.Add(new GroupOrderEvent
        {
            Id = Guid.NewGuid(),
            GroupOrderId = order.Id,
            EventType = "DraftCreated",
            NotesEn = $"{buyer.BusinessName ?? "A buyer"} saved a draft",
            NotesAr = $"حفظ {buyer.BusinessName ?? "المتجر"} مسودة",
            CreatedBy = userId
        });

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(order);
    }

    public async Task<PaginatedResult<OrderListDto>> GetDraftsAsync(Guid userId, int page = 1, int limit = 20, CancellationToken cancellationToken = default)
    {
        return await GetOrdersAsync(userId, OrderStatus.Draft, page, limit, cancellationToken);
    }

    public async Task DeleteDraftAsync(Guid orderId, Guid userId, CancellationToken cancellationToken = default)
    {
        var order = await _groupOrderRepository.GetByIdAsync(orderId, cancellationToken)
            ?? throw new KeyNotFoundException("Draft not found.");

        var buyer = await _buyerRepository.GetByUserIdAsync(userId, cancellationToken);
        if (order.CreatorId != buyer?.Id || order.Status != OrderStatus.Draft)
            throw new InvalidOperationException("Cannot delete this draft.");

        _groupOrderRepository.Delete(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<object> JoinOrderAsync(Guid orderId, Guid userId, JoinOrderRequest request, CancellationToken cancellationToken = default)
    {
        var order = await _groupOrderRepository.GetWithDetailsAsync(orderId, cancellationToken)
            ?? throw new KeyNotFoundException("Order not found.");

        if (order.Status != OrderStatus.Open)
            throw new InvalidOperationException("Order is not open for joining.");

        if (DateTimeOffset.UtcNow > order.DeadlineAt)
            throw new InvalidOperationException("Order deadline has passed.");

        var buyer = await _buyerRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("Buyer profile not found.");

        var participant = await _participantRepository.FindOneAsync(p => p.GroupOrderId == orderId && p.BuyerId == buyer.Id, cancellationToken);

        bool isFreshJoin;
        if (participant == null)
        {
            isFreshJoin = true;
            participant = new GroupOrderParticipant
            {
                Id = Guid.NewGuid(),
                GroupOrderId = orderId,
                BuyerId = buyer.Id,
                JoinedAt = DateTimeOffset.UtcNow,
                Status = "Joined"
            };
            _participantRepository.Add(participant);
            _eventRepository.Add(new GroupOrderEvent
            {
                Id = Guid.NewGuid(),
                GroupOrderId = order.Id,
                EventType = "BuyerJoined",
                NotesEn = $"{buyer.User?.FullName ?? "A buyer"} joined the order",
                NotesAr = $"انضم {buyer.User?.FullName ?? "مشتري"} إلى الطلب",
                CreatedBy = userId
            });
        }
        else if (participant.Status != "Joined")
        {
            isFreshJoin = true;
            participant.Status = "Joined";
            participant.JoinedAt = DateTimeOffset.UtcNow;
            participant.CancelledAt = null;
            _participantRepository.Update(participant);
            _eventRepository.Add(new GroupOrderEvent
            {
                Id = Guid.NewGuid(),
                GroupOrderId = order.Id,
                EventType = "BuyerJoined",
                NotesEn = $"{buyer.User?.FullName ?? "A buyer"} rejoined the order",
                NotesAr = $"انضم {buyer.User?.FullName ?? "مشتري"} مجدداً إلى الطلب",
                CreatedBy = userId
            });
        }
        else
        {
            isFreshJoin = false;
        }

        var allowedCategoryIds = order.Items?
            .Where(i => i.Product != null)
            .Select(i => i.Product!.CategoryId)
            .ToHashSet() ?? [];

        // Capture qty-before state to detect tier changes
        var qtyBefore = order.Items?.ToDictionary(i => i.Id, i => i.TargetQty) ?? [];

        // For existing participants, delete old items and recreate fresh
        var oldItemsList = new List<ParticipantItem>();
        if (!isFreshJoin)
        {
            var oldItems = await _participantItemRepository.GetByParticipantAsync(participant.Id, cancellationToken);
            oldItemsList = oldItems.ToList();
            foreach (var old in oldItemsList)
                _participantItemRepository.Delete(old);
        }

        var productToGoItem = new Dictionary<Guid, GroupOrderItem>();

        foreach (var item in request.Items)
        {
            if (!productToGoItem.TryGetValue(item.ProductId, out var goItem))
            {
                goItem = order.Items?.FirstOrDefault(i => i.ProductId == item.ProductId);
                if (goItem == null)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId, cancellationToken)
                        ?? throw new InvalidOperationException("Product not found.");

                    if (!allowedCategoryIds.Contains(product.CategoryId))
                        throw new InvalidOperationException(
                            $"Product '{product.Name}' belongs to a category not in this order. You can only add products from the same categories as the existing order.");

                    goItem = new GroupOrderItem
                    {
                        Id = Guid.NewGuid(),
                        GroupOrderId = order.Id,
                        ProductId = item.ProductId,
                        TargetQty = item.Quantity
                    };
                    _groupOrderItemRepository.Add(goItem);
                }
                else
                {
                    int oldUserQty = oldItemsList.FirstOrDefault(o => o.GroupOrderItemId == goItem.Id)?.Quantity ?? 0;
                    int otherParticipantsQty = order.Participants?
                        .Where(p => p.Status == "Joined" && p.Id != participant.Id)
                        .SelectMany(p => p.Items ?? Enumerable.Empty<ParticipantItem>())
                        .Where(pi => pi.GroupOrderItemId == goItem.Id)
                        .Sum(pi => pi.Quantity) ?? 0;

                    int delta = item.Quantity - oldUserQty;
                    int newTargetQty = goItem.TargetQty + delta;

                    int minTargetQty = otherParticipantsQty + item.Quantity;
                    if (newTargetQty < minTargetQty)
                        newTargetQty = minTargetQty;

                    if (newTargetQty < 0) newTargetQty = 0;

                    if (goItem.TargetQty != newTargetQty)
                    {
                        goItem.TargetQty = newTargetQty;
                        _groupOrderItemRepository.Update(goItem);
                    }
                }
                productToGoItem[item.ProductId] = goItem;
            }

            _participantItemRepository.Add(new ParticipantItem
            {
                Id = Guid.NewGuid(),
                ParticipantId = participant.Id,
                GroupOrderItemId = goItem.Id,
                Quantity = item.Quantity,
                UnitPriceAtJoin = goItem.UnitPrice ?? 0
            });
        }

        // Handle products the participant completely removed from their cart
        var requestedProductIds = request.Items.Select(i => i.ProductId).ToHashSet();
        foreach (var oldItem in oldItemsList)
        {
            var goi = oldItem.GroupOrderItem;
            if (goi != null && !requestedProductIds.Contains(goi.ProductId))
            {
                goi.TargetQty = Math.Max(0, goi.TargetQty - oldItem.Quantity);
                _groupOrderItemRepository.Update(goi);
            }
        }

        // --- ItemsUpdated event (only for existing participants who update quantities) ---
        if (!isFreshJoin)
        {
            var enChanges = new List<string>();
            var arChanges = new List<string>();

            foreach (var item in request.Items)
            {
                if (!productToGoItem.TryGetValue(item.ProductId, out var goItem)) continue;
                var productName = goItem.Product?.Name ?? "Unknown";
                var oldQty = oldItemsList.FirstOrDefault(o => o.GroupOrderItemId == goItem.Id)?.Quantity;

                if (oldQty == null)
                {
                    enChanges.Add($"{productName} increased by ?{item.Quantity}");
                    arChanges.Add(ToArabicNumerals($"أضاف {productName} بكمية {item.Quantity}"));
                }
                else if (oldQty.Value != item.Quantity)
                {
                    int diff = item.Quantity - oldQty.Value;
                    if (diff > 0)
                    {
                        enChanges.Add($"{productName} increased by ?{diff}");
                        arChanges.Add(ToArabicNumerals($"زاد {productName} بكمية {diff}"));
                    }
                    else
                    {
                        enChanges.Add($"{productName} decreased by ?{-diff}");
                        arChanges.Add(ToArabicNumerals($"خفض {productName} بكمية {-diff}"));
                    }
                }
            }

            foreach (var oldItem in oldItemsList)
            {
                var goi = oldItem.GroupOrderItem;
                if (goi == null || requestedProductIds.Contains(goi.ProductId)) continue;
                var productName = goi.Product?.Name ?? "Unknown";
                enChanges.Add($"removed {productName}");
                arChanges.Add($"أزال {productName}");
            }

            if (enChanges.Count > 0)
            {
                _eventRepository.Add(new GroupOrderEvent
                {
                    Id = Guid.NewGuid(),
                    GroupOrderId = order.Id,
                    EventType = "ItemsUpdated",
                    NotesEn = $"{buyer.User?.FullName ?? "A buyer"} updated items: {string.Join(", ", enChanges)}",
                    NotesAr = $"{buyer.User?.FullName ?? "مشتري"} قام بتحديث الطلب: {string.Join("، ", arChanges)}",
                    CreatedBy = userId
                });
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // --- Notifications ---
        var creatorParticipant = order.Participants?.FirstOrDefault(p => p.BuyerId == order.CreatorId);
        var creatorUserId = order.Creator?.UserId;

        // 1. Notify the order creator that someone joined (only on a fresh join, not an update)
        if (isFreshJoin && creatorUserId.HasValue && creatorUserId.Value != userId)
        {
            _notificationRepository.Add(new Notification
            {
                Id = Guid.NewGuid(),
                UserId = creatorUserId.Value,
                Type = "BuyerJoinedOrder",
                TitleAr = "انضم مشتري لطلبك",
                TitleEn = "A Buyer Joined Your Order",
                BodyAr = $"انضم '{buyer.User?.FullName ?? "مشتري"}' إلى طلبك الجماعي '{order.Title}'.",
                BodyEn = $"'{buyer.User?.FullName ?? "A buyer"}' has joined your group order '{order.Title}'.",
                Channel = "InApp",
                RelatedOrderId = orderId
            });
        }

        // 2. Notify all other participants if a quantity change unlocked/changed a price tier
        var allParticipantUserIds = order.Participants?
            .Where(p => p.Status == "Joined" && p.BuyerId != buyer.Id && p.Buyer?.UserId != null)
            .Select(p => p.Buyer!.UserId)
            .ToList() ?? [];

        bool anyTierChanged = productToGoItem.Values
            .Any(goItem => qtyBefore.TryGetValue(goItem.Id, out var oldQty) && oldQty != goItem.TargetQty);

        if (anyTierChanged && allParticipantUserIds.Count > 0)
        {
            foreach (var participantUserId in allParticipantUserIds)
            {
                _notificationRepository.Add(new Notification
                {
                    Id = Guid.NewGuid(),
                    UserId = participantUserId,
                    Type = "OrderQuantityUpdated",
                    TitleAr = "تم تحديث كميات الطلب الجماعي",
                    TitleEn = "Group Order Quantities Updated",
                    BodyAr = $"تم تحديث الكميات في الطلب الجماعي '{order.Title}'. تحقق من الأسعار الجديدة!",
                    BodyEn = $"Quantities in group order '{order.Title}' have been updated. Check for new pricing!",
                    Channel = "InApp",
                    RelatedOrderId = orderId
                });
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updatedProducts = request.Items.Select(i =>
        {
            var goItem = productToGoItem[i.ProductId];
            return new { itemId = goItem.Id.ToString(), productId = goItem.ProductId.ToString() };
        }).ToList();

        var msg = isFreshJoin ? "Joined order successfully" : "Items updated successfully";
        return new
        {
            message = msg,
            participant = new
            {
                id = participant.Id.ToString(),
                name = buyer.User?.FullName ?? "",
                items = request.Items.Select(i => new { productId = i.ProductId.ToString(), quantity = i.Quantity })
            },
            updatedProducts
        };
    }

    public async Task<object> LeaveOrderAsync(Guid orderId, Guid userId, CancellationToken cancellationToken = default)
    {
        var buyer = await _buyerRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("Buyer profile not found.");

        var participant = await _participantRepository.FindOneAsync(
            p => p.GroupOrderId == orderId && p.BuyerId == buyer.Id && p.Status == "Joined", cancellationToken)
            ?? throw new InvalidOperationException("Not a participant in this order.");

        participant.Status = "Cancelled";
        participant.CancelledAt = DateTimeOffset.UtcNow;
        _participantRepository.Update(participant);

        var order = await _groupOrderRepository.GetWithDetailsAsync(orderId, cancellationToken);
        var items = await _participantItemRepository.GetByParticipantAsync(participant.Id, cancellationToken);

        if (order != null && items.Any())
        {
            foreach (var oldItem in items)
            {
                var goItem = order.Items?.FirstOrDefault(i => i.Id == oldItem.GroupOrderItemId);
                if (goItem != null)
                {
                    int otherParticipantsQty = order.Participants?
                        .Where(p => p.Status == "Joined" && p.Id != participant.Id)
                        .SelectMany(p => p.Items ?? Enumerable.Empty<ParticipantItem>())
                        .Where(pi => pi.GroupOrderItemId == goItem.Id)
                        .Sum(pi => pi.Quantity) ?? 0;

                    int newTargetQty = goItem.TargetQty - oldItem.Quantity;
                    if (newTargetQty < otherParticipantsQty)
                        newTargetQty = otherParticipantsQty;
                    if (newTargetQty < 0) newTargetQty = 0;

                    if (goItem.TargetQty != newTargetQty)
                    {
                        goItem.TargetQty = newTargetQty;
                        _groupOrderItemRepository.Update(goItem);
                    }

                    _participantItemRepository.Delete(oldItem);

                    if (goItem.TargetQty == 0 && otherParticipantsQty == 0)
                    {
                        _groupOrderItemRepository.Delete(goItem);
                    }
                }
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Notify the order creator that a buyer left
        if (order != null)
        {
            var creatorUserId = order.Creator?.UserId;
            if (creatorUserId.HasValue && creatorUserId.Value != userId)
            {
                _notificationRepository.Add(new Notification
                {
                    Id = Guid.NewGuid(),
                    UserId = creatorUserId.Value,
                    Type = "BuyerLeftOrder",
                    TitleAr = "غادر مشتري طلبك الجماعي",
                    TitleEn = "A Buyer Left Your Order",
                    BodyAr = $"غادر '{buyer.User?.FullName ?? "مشتري"}' طلبك الجماعي '{order.Title}'.",
                    BodyEn = $"'{buyer.User?.FullName ?? "A buyer"}' has left your group order '{order.Title}'.",
                    Channel = "InApp",
                    RelatedOrderId = orderId
                });
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }

        return new { message = "Left order successfully" };
    }

    public async Task<object> UpdateItemsAsync(Guid orderId, Guid participantId, Guid userId, UpdateItemsRequest request, CancellationToken cancellationToken = default)
    {
        var buyer = await _buyerRepository.GetByUserIdAsync(userId, cancellationToken);
        var participant = await _participantRepository.GetByIdAsync(participantId, cancellationToken)
            ?? throw new KeyNotFoundException("Participant not found.");

        if (participant.BuyerId != buyer?.Id)
            throw new UnauthorizedAccessException("Not your participation.");

        var existingItems = await _participantItemRepository.GetByParticipantAsync(participantId, cancellationToken);

        foreach (var item in request.Items)
        {
            var goItem = await _groupOrderItemRepository.FindOneAsync(i => i.GroupOrderId == orderId && i.Id == item.GroupOrderItemId, cancellationToken);
            if (goItem == null) continue;

            var existing = existingItems.FirstOrDefault(e => e.GroupOrderItemId == goItem.Id);

            if (existing != null)
            {
                existing.Quantity = item.Quantity;
                _participantItemRepository.Update(existing);
            }
            else
            {
                _participantItemRepository.Add(new ParticipantItem
                {
                    Id = Guid.NewGuid(),
                    ParticipantId = participantId,
                    GroupOrderItemId = goItem.Id,
                    Quantity = item.Quantity,
                    UnitPriceAtJoin = goItem.UnitPrice ?? 0
                });
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new { message = "Quantities updated" };
    }

    private async Task<Guid> ResolveGroupRegionAsync(Guid buyerRegionId, CancellationToken cancellationToken)
    {
        var allowedTypes = await GetAllowedGroupRegionTypesAsync(cancellationToken);
        if (allowedTypes.Count == 0)
            return buyerRegionId;

        var matched = await _regionRepository.FindMatchingAncestorAsync(buyerRegionId, allowedTypes, cancellationToken);
        return matched?.Id ?? throw new InvalidOperationException(
            "No matching region found in hierarchy for the allowed group region types.");
    }

    private async Task<List<RegionType>> GetAllowedGroupRegionTypesAsync(CancellationToken cancellationToken)
    {
        var setting = await _appSettingRepository.GetByKeyAsync("GroupRegionTypes", cancellationToken);
        if (setting is null || string.IsNullOrWhiteSpace(setting.Value))
            return [];

        try
        {
            var typeNames = JsonSerializer.Deserialize<List<string>>(setting.Value) ?? [];
            return typeNames
                .Select(n => Enum.TryParse<RegionType>(n, true, out var t) ? t : (RegionType?)null)
                .Where(t => t.HasValue)
                .Select(t => t!.Value)
                .ToList();
        }
        catch
        {
            return [];
        }
    }

    public async Task<IReadOnlyList<EligibleSupplierDto>> GetEligibleSuppliersAsync(Guid orderId, Guid userId, CancellationToken cancellationToken = default)
    {
        var order = await _groupOrderRepository.GetWithDetailsAsync(orderId, cancellationToken)
            ?? throw new KeyNotFoundException("Order not found.");

        if (order.Items == null || order.Items.Count == 0) return [];

        var orderItems = order.Items.ToList();
        var productIds = orderItems.Select(i => i.ProductId).ToList();
        var supplierProducts = await _supplierProductRepository.GetForProductsWithTiersAsync(productIds, cancellationToken);

        var eligible = new List<EligibleSupplierDto>();

        var grouped = supplierProducts.GroupBy(sp => sp.SupplierId);
        foreach (var g in grouped)
        {
            var firstSp = g.First();
            var supplier = firstSp.Supplier;
            if (supplier == null) continue;

            decimal totalCost = 0;
            var coveredProducts = new List<EligibleProductDto>();

            foreach (var req in orderItems)
            {
                var sp = g.FirstOrDefault(x => x.ProductId == req.ProductId);
                if (sp == null || sp.Stock < req.TargetQty) continue;

                decimal unitPrice = sp.Price;
                var tiers = new List<EligiblePricingTierDto>();

                if (sp.PricingTiers != null && sp.PricingTiers.Any())
                {
                    var applicableTier = sp.PricingTiers
                        .Where(t => req.TargetQty >= t.MinQty && (t.MaxQty == null || req.TargetQty <= t.MaxQty))
                        .OrderByDescending(t => t.MinQty)
                        .FirstOrDefault();

                    if (applicableTier != null)
                        unitPrice = applicableTier.UnitPrice;

                    tiers = sp.PricingTiers.Select(t => new EligiblePricingTierDto
                    {
                        MinQty = t.MinQty,
                        MaxQty = t.MaxQty,
                        UnitPrice = t.UnitPrice
                    }).ToList();
                }

                totalCost += unitPrice * req.TargetQty;
                coveredProducts.Add(new EligibleProductDto
                {
                    ProductId = req.ProductId,
                    GroupOrderItemId = req.Id,
                    UnitPrice = unitPrice,
                    AvailableStock = sp.Stock,
                    PricingTiers = tiers
                });
            }

            if (coveredProducts.Count > 0)
            {
                eligible.Add(new EligibleSupplierDto
                {
                    SupplierId = supplier.Id,
                    SupplierName = supplier.CompanyName,
                    TotalEstimatedCost = totalCost,
                    CoveredProductCount = coveredProducts.Count,
                    CoveredProducts = coveredProducts
                });
            }
        }

        return eligible.OrderByDescending(e => e.CoveredProductCount).ThenBy(e => e.TotalEstimatedCost).ToList();
    }

    public async Task<object> UpdateOrderItemsAsync(Guid orderId, Guid userId, List<CreateOrderItem> items, CancellationToken cancellationToken = default)
    {
        var order = await _groupOrderRepository.GetWithDetailsAsync(orderId, cancellationToken)
            ?? throw new KeyNotFoundException("Order not found.");

        var buyer = await _buyerRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("Buyer profile not found.");

        if (order.CreatorId != buyer.Id)
            throw new UnauthorizedAccessException("Only the creator can modify order items.");

        if (order.Status != OrderStatus.Open && order.Status != OrderStatus.Draft)
            throw new InvalidOperationException("Cannot modify items on a closed/completed/cancelled order.");

        if (DateTimeOffset.UtcNow > order.DeadlineAt)
            throw new InvalidOperationException("Order deadline has passed. Cannot modify items.");

        var requestProductIds = items.Select(i => i.ProductId).ToHashSet();
        var currentItems = order.Items?.ToList() ?? [];
        var currentProductIds = currentItems.Select(i => i.ProductId).ToHashSet();

        // Items to remove (exist in current order but not in the request)
        var toRemove = currentItems.Where(i => !requestProductIds.Contains(i.ProductId)).ToList();
        foreach (var item in toRemove)
        {
            // Cascade-delete related ParticipantItems first (FK is Restrict)
            if (item.ParticipantItems != null)
            {
                foreach (var pi in item.ParticipantItems.ToList())
                    _participantItemRepository.Delete(pi);
            }
            _groupOrderItemRepository.Delete(item);
        }

        // Items to add or update
        foreach (var req in items)
        {
            var existing = currentItems.FirstOrDefault(i => i.ProductId == req.ProductId);
            if (existing != null)
            {
                int committedQty = existing.ParticipantItems?.Sum(pi => pi.Quantity) ?? 0;
                if (req.TargetQuantity < committedQty)
                    throw new InvalidOperationException($"Cannot set target quantity below {committedQty} ? participants have already committed that amount.");
                existing.TargetQty = req.TargetQuantity;
                _groupOrderItemRepository.Update(existing);
            }
            else
            {
                _groupOrderItemRepository.Add(new GroupOrderItem
                {
                    Id = Guid.NewGuid(),
                    GroupOrderId = order.Id,
                    ProductId = req.ProductId,
                    TargetQty = req.TargetQuantity,
                    ItemStatus = "Unassigned"
                });
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new { message = "Order items updated successfully" };
    }

    
    public async Task<object> AssignSupplierItemsAsync(Guid orderId, AssignSupplierItemsRequest request, Guid userId, CancellationToken cancellationToken = default)
    {
        var order = await _groupOrderRepository.GetWithDetailsAsync(orderId, cancellationToken)
            ?? throw new KeyNotFoundException("Order not found.");

        var buyer = await _buyerRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("Buyer profile not found.");

        if (order.CreatorId != buyer.Id)
            throw new UnauthorizedAccessException("Only the order creator can assign a supplier.");

        if (order.Status != OrderStatus.Open)
            throw new InvalidOperationException("Order must be open to assign a supplier.");

        if (DateTimeOffset.UtcNow <= order.DeadlineAt)
            throw new InvalidOperationException("Cannot assign a supplier before the order deadline passes.");

        var supplierProducts = await _supplierProductRepository.GetBySupplierWithTiersAsync(request.SupplierId, cancellationToken);
        var assignedItems = new List<string>();
        var itemNames = new List<string>();

        foreach (var itemId in request.ItemIds)
        {
            var item = order.Items?.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
                throw new InvalidOperationException($"Order item {itemId} not found.");

            if (item.ItemStatus != "Unassigned")
                throw new InvalidOperationException($"Item '{item.Product?.Name}' is already assigned to a supplier.");

            var sp = supplierProducts.FirstOrDefault(p => p.ProductId == item.ProductId);
            if (sp == null)
                throw new InvalidOperationException($"The supplier does not offer the product '{item.Product?.Name}'.");

            if (sp.Stock < item.TargetQty)
                throw new InvalidOperationException($"The supplier does not have enough stock for '{item.Product?.Name}' (needed: {item.TargetQty}, available: {sp.Stock}).");

            decimal unitPrice = sp.Price;
            if (sp.PricingTiers != null && sp.PricingTiers.Any())
            {
                var applicableTier = sp.PricingTiers
                    .Where(t => item.TargetQty >= t.MinQty && (t.MaxQty == null || item.TargetQty <= t.MaxQty))
                    .OrderByDescending(t => t.MinQty)
                    .FirstOrDefault();
                if (applicableTier != null)
                    unitPrice = applicableTier.UnitPrice;
            }

            item.SupplierId = request.SupplierId;
            item.SupplierProductId = sp.Id;
            item.UnitPrice = unitPrice;
            item.ItemStatus = "Pending";
            _groupOrderItemRepository.Update(item);

            assignedItems.Add(item.Id.ToString());
            itemNames.Add(item.Product?.Name ?? "Unknown");
        }

        if (assignedItems.Count == 0)
            throw new InvalidOperationException("No items were assigned.");

        var delivery = new Delivery
        {
            Id = Guid.NewGuid(),
            GroupOrderId = order.Id,
            SupplierId = request.SupplierId,
            Status = "Pending",
            DeliveryFee = 0,
            DeliveryType = "System",
            ShippingRegion = order.Region?.NameEn ?? ""
        };
        _deliveryRepository.Add(delivery);

        var supplier = await _supplierRepository.GetByIdAsync(request.SupplierId, cancellationToken);
        var supplierName = supplier?.CompanyName ?? "Supplier";

        _eventRepository.Add(new GroupOrderEvent
        {
            Id = Guid.NewGuid(),
            GroupOrderId = order.Id,
            EventType = "SupplierAssigned",
            NotesEn = $"{supplierName} assigned to items: {string.Join(", ", itemNames)}",
            NotesAr = $"\u062a\u0645 \u062a\u0639\u064a\u064a\u0646 {supplierName} \u0644\u0644\u0639\u0646\u0627\u0635\u0631: {string.Join('\u060c', itemNames)}",
            CreatedBy = userId
        });

        if (supplier?.UserId is Guid supplierUserId)
        {
            _notificationRepository.Add(new Notification
            {
                Id = Guid.NewGuid(),
                UserId = supplierUserId,
                Type = "SupplierAssignedOrder",
                TitleAr = "\u0637\u0644\u0628 \u062c\u062f\u064a\u062f \u0644\u0643",
                TitleEn = "New Order Assignment",
                BodyAr = $"\u062a\u0645 \u062a\u0639\u064a\u064a\u0646\u0643 \u0644\u0637\u0644\u0628 '{order.Title}'. \u0627\u0644\u0639\u0646\u0627\u0635\u0631: {string.Join('\u060c', itemNames)}",
                BodyEn = $"You have been assigned to order '{order.Title}'. Items: {string.Join(", ", itemNames)}",
                Channel = "InApp",
                RelatedOrderId = orderId
            });
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new { message = "Supplier assigned to items.", orderStatus = order.Status, assignedItems, supplierId = request.SupplierId };
    }

    private async Task AutoAssignDeliveryPersonAsync(GroupOrder order, Guid supplierId, CancellationToken cancellationToken)
    {
        var regionId = order.Region?.Id ?? order.RegionId;
        var profiles = await _deliveryPersonProfileRepository.GetForRegionAsync(regionId, cancellationToken);
        var available = profiles.Where(p => p.IsActive).ToList();

        if (available.Count == 0) return;

        // Pick the delivery person with the lowest fee (or highest rating as tiebreaker)
        var chosen = available
            .OrderBy(p => p.BaseDeliveryFee)
            .ThenByDescending(p => p.Rating)
            .First();

        // Create delivery record for this supplier
        var delivery = new Delivery
        {
            Id = Guid.NewGuid(),
            GroupOrderId = order.Id,
            SupplierId = supplierId,
            DeliveryPersonId = chosen.UserId,
            Status = "Pending",
            DeliveryFee = chosen.BaseDeliveryFee,
            DeliveryType = "System",
            ShippingRegion = order.Region?.NameEn ?? ""
        };
        _deliveryRepository.Add(delivery);
    }

    private static GroupOrderDto MapToDto(GroupOrder o) =>
        new(o.Id, o.CreatorId, o.RegionId, o.Title, o.Description,
            o.OrderNumber, o.DeadlineAt, o.Status,
            o.ClosedAt, o.CreatedAt, o.UpdatedAt);

    public async Task<IReadOnlyList<BuyerDeliveryDto>> GetMyDeliveriesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var buyer = await _buyerRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("Buyer profile not found.");

        var invoices = await _invoiceRepository.GetByBuyerAsync(buyer.Id, cancellationToken);
        var result = new List<BuyerDeliveryDto>();

        foreach (var invoice in invoices)
        {
            var delivery = invoice.GroupOrder?.Deliveries?.FirstOrDefault();
            if (delivery == null) continue;

            var items = invoice.Participant?.Items?
                .Where(pi => pi.Quantity > 0)
                .Select(pi => new BuyerDeliveryItemDto
                {
                    ProductName = pi.GroupOrderItem?.Product?.Name ?? "",
                    Quantity = pi.Quantity
                }).ToList() ?? [];

            result.Add(new BuyerDeliveryDto
            {
                Id = delivery.Id,
                OrderId = delivery.GroupOrderId,
                OrderTitle = invoice.GroupOrder?.Title ?? "",
                Status = delivery.Status,
                ScheduledAt = delivery.ScheduledAt,
                DeliveryPersonName = delivery.DeliveryPerson?.FullName,
                VerificationCode = invoice.VerificationCode,
                ShippingRegion = delivery.ShippingRegion,
                Items = items
            });
        }

        return result;
    }

    public async Task<object> SetDeliveryPreferenceAsync(Guid orderId, string preference, Guid? preferredDeliveryPersonId, Guid userId, CancellationToken cancellationToken = default)
    {
        // Delivery is now handled automatically by the system.
        // This method is kept as a no-op for backward compatibility.
        return new { message = "Delivery is handled automatically by the system.", preference = "SystemDelivery" };
    }

    public async Task<object> ApproveDeliveryFeeAsync(Guid orderId, bool isApproved, string? reason, Guid userId, CancellationToken cancellationToken = default)
    {
        // Delivery fees are handled automatically by the system.
        // This method is kept as a no-op for backward compatibility.
        return new { message = "Delivery fees are handled automatically by the system.", status = "Approved" };
    }

    public async Task<IReadOnlyList<DeliveryPersonProfileDto>> GetAvailableDeliveryPersonsAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        // Delivery persons are assigned automatically by the system based on coverage region.
        return [];
    }

    private static string ToArabicNumerals(string input)
    {
        return input
            .Replace('0', '\u0660')
            .Replace('1', '\u0661')
            .Replace('2', '\u0662')
            .Replace('3', '\u0663')
            .Replace('4', '\u0664')
            .Replace('5', '\u0665')
            .Replace('6', '\u0666')
            .Replace('7', '\u0667')
            .Replace('8', '\u0668')
            .Replace('9', '\u0669');
    }

    private async Task<int> GetDefaultDeadlineDaysAsync(CancellationToken cancellationToken)
    {
        var setting = await _appSettingRepository.GetByKeyAsync("DefaultDeadlineDays", cancellationToken);
        if (setting is null || !int.TryParse(setting.Value, out var days))
            return 3;
        return days;
    }

    private async Task<int> GetUrgentDeadlineHoursAsync(CancellationToken cancellationToken)
    {
        var setting = await _appSettingRepository.GetByKeyAsync("UrgentDeadlineHours", cancellationToken);
        if (setting is null || !int.TryParse(setting.Value, out var hours))
            return 6;
        return hours;
    }
}

