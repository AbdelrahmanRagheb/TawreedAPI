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

        // Same supplier-matching algorithm as GetOrderDetailAsync (lines 185-243)
        var orderTotals = new Dictionary<Guid, decimal>();
        var ordersNeedingPrices = paged
            .Where(o => o.SupplierId == null && o.Items != null && o.Items.Count > 0)
            .ToList();

        if (ordersNeedingPrices.Count > 0)
        {
            var allPids = ordersNeedingPrices
                .SelectMany(o => o.Items!.Select(i => i.ProductId))
                .Distinct()
                .ToList();

            var allSps = await _supplierProductRepository.GetForProductsWithTiersAsync(allPids, cancellationToken);
            var bySupplier = allSps.GroupBy(sp => sp.SupplierId).ToList();

            foreach (var order in ordersNeedingPrices)
            {
                var productIds = order.Items!.Select(i => i.ProductId).ToHashSet();
                decimal lowestTotalCost = decimal.MaxValue;

                foreach (var group in bySupplier)
                {
                    var firstSp = group.First();
                    var supplier = firstSp.Supplier;
                    if (supplier == null) continue;

                    var groupProductIds = group.Select(sp => sp.ProductId).ToHashSet();
                    if (!productIds.All(pid => groupProductIds.Contains(pid))) continue;

                    bool canFulfill = true;
                    decimal totalCost = 0;

                    foreach (var item in order.Items!)
                    {
                        var sp = group.FirstOrDefault(x => x.ProductId == item.ProductId);
                        if (sp == null || sp.Stock < item.TargetQty)
                        {
                            canFulfill = false;
                            break;
                        }

                        decimal unitPrice = sp.Price;
                        if (sp.PricingTiers != null && sp.PricingTiers.Any())
                        {
                            var tier = sp.PricingTiers
                                .Where(t => item.TargetQty >= t.MinQty && (t.MaxQty == null || item.TargetQty <= t.MaxQty))
                                .OrderByDescending(t => t.MinQty)
                                .FirstOrDefault();
                            if (tier != null) unitPrice = tier.UnitPrice;
                        }

                        totalCost += unitPrice * item.TargetQty;
                    }

                    if (canFulfill && totalCost < lowestTotalCost)
                        lowestTotalCost = totalCost;
                }

                if (lowestTotalCost < decimal.MaxValue)
                    orderTotals[order.Id] = lowestTotalCost;
            }
        }

        var items = paged.Select(o => new OrderListDto
        {
            Id = o.Id,
            Title = o.Title,
            Status = o.Status,
            CreatedAt = o.CreatedAt,
            Deadline = o.DeadlineAt,
            TotalOrderValue = orderTotals.GetValueOrDefault(o.Id,
                o.Items?.Sum(i => (i.SupplierProduct?.Price ?? i.UnitPrice ?? 0) * i.TargetQty) ?? 0
            ),
            ParticipantCount = o.Participants?.Count(p => p.Status == "Joined") ?? 0,
            ProductCount = o.Items?.Count ?? 0,
            Region = o.Region?.NameEn ?? "",
            CreatorName = o.Creator?.User?.FullName ?? "",
            SupplierName = o.Supplier?.CompanyName ?? "",
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
            TotalOrderValue = order.Items?.Sum(i => (i.SupplierProduct?.Price ?? i.UnitPrice ?? 0) * i.TargetQty) ?? 0,
            SupplierName = order.Supplier?.CompanyName ?? "",
            SupplierId = order.SupplierId,
            DeliveryPreference = order.DeliveryPreference,
            PreferredDeliveryPersonId = order.PreferredDeliveryPersonId,
            PreferredDeliveryPersonName = order.PreferredDeliveryPerson?.User?.FullName,
            ProposedDeliveryFee = order.ProposedDeliveryFee,
            DeliveryApprovalStatus = order.DeliveryApprovalStatus,
            AssignedDeliveryPersonName = order.AssignedDeliveryPerson?.User?.FullName,
            Products = order.Items?.Select(i =>
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
                    UnitPrice = i.UnitPrice ?? i.SupplierProduct?.Price,
                    SupplierProductId = i.SupplierProductId
                };
            }).ToList() ?? [],
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

        if (order.SupplierId == null && order.Items != null && order.Items.Any())
        {
            var productIds = order.Items.Select(i => i.ProductId).ToList();
            var supplierProducts = await _supplierProductRepository.GetForProductsWithTiersAsync(productIds, cancellationToken);
            
            var grouped = supplierProducts.GroupBy(sp => sp.SupplierId);
            
            Guid? suggestedSupplierId = null;
            string suggestedSupplierName = "";
            decimal lowestTotalCost = decimal.MaxValue;
            var suggestedPrices = new Dictionary<Guid, (decimal UnitPrice, Guid SupplierProductId)>();

            foreach (var g in grouped)
            {
                var firstSp = g.First();
                var supplier = firstSp.Supplier;
                if (supplier == null) continue;
                if (g.Count() != productIds.Count) continue; // Missing some products

                bool canFulfill = true;
                decimal totalCost = 0;
                var currentPrices = new Dictionary<Guid, (decimal UnitPrice, Guid SupplierProductId)>();

                foreach (var req in order.Items)
                {
                    var sp = g.FirstOrDefault(x => x.ProductId == req.ProductId);
                    // Must have enough stock to fulfill TargetQty
                    if (sp == null || sp.Stock < req.TargetQty) 
                    {
                        canFulfill = false;
                        break;
                    }
                    
                    decimal unitPrice = sp.Price;
                    if (sp.PricingTiers != null && sp.PricingTiers.Any())
                    {
                        var applicableTier = sp.PricingTiers
                            .Where(t => req.TargetQty >= t.MinQty && (t.MaxQty == null || req.TargetQty <= t.MaxQty))
                            .OrderByDescending(t => t.MinQty)
                            .FirstOrDefault();

                        if (applicableTier != null)
                        {
                            unitPrice = applicableTier.UnitPrice;
                        }
                    }

                    totalCost += unitPrice * req.TargetQty;
                    currentPrices[req.ProductId] = (unitPrice, sp.Id);
                }

                if (canFulfill && totalCost < lowestTotalCost)
                {
                    lowestTotalCost = totalCost;
                    suggestedSupplierId = supplier.Id;
                    suggestedSupplierName = supplier.CompanyName;
                    suggestedPrices = currentPrices;
                }
            }

            if (suggestedSupplierId != null)
            {
                dto.SupplierId = suggestedSupplierId;
                dto.SupplierName = suggestedSupplierName;
                dto.TotalOrderValue = lowestTotalCost;
                foreach (var productDto in dto.Products)
                {
                    if (suggestedPrices.TryGetValue(productDto.ProductId, out var priceInfo))
                    {
                        productDto.UnitPrice = priceInfo.UnitPrice;
                        productDto.SupplierProductId = priceInfo.SupplierProductId;
                    }
                }
            }
        }
        else if (order.SupplierId != null && order.Items != null && order.Items.Any())
        {
            // Recalculate prices using the assigned supplier's pricing tiers.
            // This ensures correct pricing even if the assignment happened before
            // the tier-calculation fix in AssignSupplierAsync.
            var assignedSp = await _supplierProductRepository.GetBySupplierWithTiersAsync(order.SupplierId.Value, cancellationToken);
            
            foreach (var productDto in dto.Products)
            {
                var item = order.Items.FirstOrDefault(i => i.ProductId == productDto.ProductId);
                var sp = assignedSp.FirstOrDefault(p => p.ProductId == productDto.ProductId);
                if (sp != null && item != null)
                {
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
                    productDto.UnitPrice = unitPrice;
                    productDto.SupplierProductId = sp.Id;
                }
            }
            
            dto.TotalOrderValue = dto.Products.Sum(p => (p.UnitPrice ?? 0) * p.TargetQuantity);
        }

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
            DeadlineAt = request.Deadline,
            Status = OrderStatus.Open
        };
        _groupOrderRepository.Add(order);

        foreach (var item in request.Items)
        {
            var sp = await _supplierProductRepository.FindCheapestForProductAsync(item.ProductId, cancellationToken);
            _groupOrderItemRepository.Add(new GroupOrderItem
            {
                Id = Guid.NewGuid(),
                GroupOrderId = order.Id,
                ProductId = item.ProductId,
                TargetQty = item.TargetQuantity,
                SupplierProductId = sp?.Id,
                UnitPrice = sp?.Price
            });
        }

        _eventRepository.Add(new GroupOrderEvent
        {
            Id = Guid.NewGuid(),
            GroupOrderId = order.Id,
            EventType = "Created",
            NotesEn = $"{buyer.BusinessName ?? "A buyer"} created the order",
            NotesAr = $"أنشأ {buyer.BusinessName ?? "المشتري"} الطلب",
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
                TitleAr = "??? ????? ???? ?? ??????",
                TitleEn = "New Group Order in Your Area",
                BodyAr = $"?? ????? ??? ????? ???? ?????? '{order.Title}'. ???? ????!",
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
            DeadlineAt = request.Deadline,
            Status = OrderStatus.Draft
        };
        _groupOrderRepository.Add(order);

        foreach (var item in request.Items)
        {
            var sp = await _supplierProductRepository.FindCheapestForProductAsync(item.ProductId, cancellationToken);
            _groupOrderItemRepository.Add(new GroupOrderItem
            {
                Id = Guid.NewGuid(),
                GroupOrderId = order.Id,
                ProductId = item.ProductId,
                TargetQty = item.TargetQuantity,
                SupplierProductId = sp?.Id,
                UnitPrice = sp?.Price
            });
        }

        _eventRepository.Add(new GroupOrderEvent
        {
            Id = Guid.NewGuid(),
            GroupOrderId = order.Id,
            EventType = "DraftCreated",
            NotesEn = $"{buyer.BusinessName ?? "A buyer"} saved a draft",
            NotesAr = $"حفظ {buyer.BusinessName ?? "المشتري"} مسودة",
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
                    enChanges.Add($"{productName} increased by �{item.Quantity}");
                    arChanges.Add(ToArabicNumerals($"زيادة {productName} بمقدار {item.Quantity}"));
                }
                else if (oldQty.Value != item.Quantity)
                {
                    int diff = item.Quantity - oldQty.Value;
                    if (diff > 0)
                    {
                        enChanges.Add($"{productName} increased by �{diff}");
                        arChanges.Add(ToArabicNumerals($"زيادة {productName} بمقدار {diff}"));
                    }
                    else
                    {
                        enChanges.Add($"{productName} decreased by �{-diff}");
                        arChanges.Add(ToArabicNumerals($"نقص {productName} بمقدار {-diff}"));
                    }
                }
            }

            foreach (var oldItem in oldItemsList)
            {
                var goi = oldItem.GroupOrderItem;
                if (goi == null || requestedProductIds.Contains(goi.ProductId)) continue;
                var productName = goi.Product?.Name ?? "Unknown";
                enChanges.Add($"removed {productName}");
                arChanges.Add($"إزالة {productName}");
            }

            if (enChanges.Count > 0)
            {
                _eventRepository.Add(new GroupOrderEvent
                {
                    Id = Guid.NewGuid(),
                    GroupOrderId = order.Id,
                    EventType = "ItemsUpdated",
                    NotesEn = $"{buyer.User?.FullName ?? "A buyer"} updated items: {string.Join(", ", enChanges)}",
                    NotesAr = $"{buyer.User?.FullName ?? "مشتري"} قام بتحديث العناصر: {string.Join("، ", arChanges)}",
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
                TitleAr = "???? ????? ???? ?????",
                TitleEn = "A Buyer Joined Your Order",
                BodyAr = $"???? '{buyer.User?.FullName ?? "?????"}' ??? ???? ??????? '{order.Title}'.",
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
                    TitleAr = "????? ????? ????? ???????",
                    TitleEn = "Group Order Quantities Updated",
                    BodyAr = $"?? ????? ????? ????? ??????? '{order.Title}'. ???? ?? ??????? ???????!",
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
                    TitleAr = "????? ???? ???? ???????",
                    TitleEn = "A Buyer Left Your Order",
                    BodyAr = $"???? '{buyer.User?.FullName ?? "?????"}' ???? ??????? '{order.Title}'.",
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

        var productIds = order.Items.Select(i => i.ProductId).ToList();

        var supplierProducts = await _supplierProductRepository.GetForProductsWithTiersAsync(productIds, cancellationToken);

        var eligible = new List<EligibleSupplierDto>();

        var grouped = supplierProducts.GroupBy(sp => sp.SupplierId);
        foreach (var g in grouped)
        {
            var firstSp = g.First();
            var supplier = firstSp.Supplier;
            if (supplier == null) continue;
            if (g.Count() != productIds.Count) continue; // Missing some products

            bool canFulfill = true;
            decimal totalCost = 0;

            foreach (var req in order.Items)
            {
                var sp = g.FirstOrDefault(x => x.ProductId == req.ProductId);
                // Must have enough stock to fulfill TargetQty
                if (sp == null || sp.Stock < req.TargetQty) 
                {
                    canFulfill = false;
                    break;
                }
                
                decimal unitPrice = sp.Price;
                if (sp.PricingTiers != null && sp.PricingTiers.Any())
                {
                    var applicableTier = sp.PricingTiers
                        .Where(t => req.TargetQty >= t.MinQty && (t.MaxQty == null || req.TargetQty <= t.MaxQty))
                        .OrderByDescending(t => t.MinQty)
                        .FirstOrDefault();

                    if (applicableTier != null)
                    {
                        unitPrice = applicableTier.UnitPrice;
                    }
                }

                totalCost += unitPrice * req.TargetQty;
            }

            if (canFulfill)
            {
                eligible.Add(new EligibleSupplierDto
                {
                    SupplierId = supplier.Id,
                    SupplierName = supplier.CompanyName,
                    Rating = supplier.RatingAvg,
                    TotalEstimatedCost = totalCost
                });
            }
        }

        return eligible.OrderBy(e => e.TotalEstimatedCost).ToList();
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
                    throw new InvalidOperationException($"Cannot set target quantity below {committedQty} � participants have already committed that amount.");
                existing.TargetQty = req.TargetQuantity;
                _groupOrderItemRepository.Update(existing);
            }
            else
            {
                var sp = await _supplierProductRepository.FindCheapestForProductAsync(req.ProductId, cancellationToken);
                _groupOrderItemRepository.Add(new GroupOrderItem
                {
                    Id = Guid.NewGuid(),
                    GroupOrderId = order.Id,
                    ProductId = req.ProductId,
                    TargetQty = req.TargetQuantity,
                    SupplierProductId = sp?.Id,
                    UnitPrice = sp?.Price
                });
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new { message = "Order items updated successfully" };
    }

    public async Task<object> AssignSupplierAsync(Guid orderId, Guid supplierId, Guid userId, CancellationToken cancellationToken = default)
    {
        var order = await _groupOrderRepository.GetWithDetailsAsync(orderId, cancellationToken)
            ?? throw new KeyNotFoundException("Order not found.");

        var buyer = await _buyerRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("Buyer profile not found.");

        if (order.CreatorId != buyer.Id)
            throw new UnauthorizedAccessException("Only the order creator can assign a supplier.");

        if (order.Status != OrderStatus.Open)
            throw new InvalidOperationException("Order must be open to assign a supplier.");

        if (order.SupplierId.HasValue)
            throw new InvalidOperationException("A supplier is already assigned to this order.");

        if (DateTimeOffset.UtcNow <= order.DeadlineAt)
            throw new InvalidOperationException("Cannot assign a supplier before the order deadline passes.");

        order.SupplierId = supplierId;
        order.Status = OrderStatus.PendingApproval;

        // Ensure the supplier offers all products in the order
        var supplierProducts = await _supplierProductRepository.GetBySupplierWithTiersAsync(supplierId, cancellationToken);
        foreach (var item in order.Items ?? [])
        {
            var sp = supplierProducts.FirstOrDefault(p => p.ProductId == item.ProductId);
            if (sp == null)
                throw new InvalidOperationException($"The supplier does not offer the product '{item.Product?.Name}' (ID: {item.ProductId}). Please choose a supplier that covers all order items.");

            // Calculate applicable pricing tier based on target quantity
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
            item.UnitPrice = unitPrice;
            item.SupplierProductId = sp.Id;
        }

        _eventRepository.Add(new GroupOrderEvent
        {
            Id = Guid.NewGuid(),
            GroupOrderId = order.Id,
            EventType = "SupplierAssigned",
            NotesEn = "A supplier has been assigned to this order",
            NotesAr = "تم تعيين مورد لهذا الطلب",
            CreatedBy = userId
        });

        var supplier = await _supplierRepository.GetByIdAsync(supplierId, cancellationToken);
        if (supplier?.UserId is Guid supplierUserId)
        {
            _notificationRepository.Add(new Notification
            {
                Id = Guid.NewGuid(),
                UserId = supplierUserId,
                Type = "SupplierAssignedOrder",
                TitleAr = "????? ??? ????",
                TitleEn = "New Order Assignment",
                BodyAr = $"?? ?????? ???? '{order.Title}' � ???? ???????? ??????? ?? ?????.",
                BodyEn = $"You have been assigned to order '{order.Title}' � please review and accept or decline.",
                Channel = "InApp",
                RelatedOrderId = orderId
            });
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new { message = "Supplier assigned. Waiting for supplier approval.", orderStatus = OrderStatus.PendingApproval, supplierId };
    }

    private static GroupOrderDto MapToDto(GroupOrder o) =>
        new(o.Id, o.CreatorId, o.SupplierId, o.RegionId, o.Title, o.Description,
            o.OrderNumber, o.Notes, o.Visibility, o.DeadlineAt, o.Status,
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
        var order = await _groupOrderRepository.GetWithDetailsAsync(orderId, cancellationToken)
            ?? throw new KeyNotFoundException("Order not found.");

        if (order.Creator?.UserId != userId)
            throw new UnauthorizedAccessException("Only the order creator can set delivery preferences.");

        var validPreferences = new[] { "None", "OwnDelivery", "SystemDelivery", "SpecificPerson" };
        if (!validPreferences.Contains(preference))
            throw new InvalidOperationException($"Invalid preference '{preference}'. Valid values: {string.Join(", ", validPreferences)}");

        order.DeliveryPreference = preference;
        order.PreferredDeliveryPersonId = preference == "SpecificPerson" ? preferredDeliveryPersonId : null;
        _groupOrderRepository.Update(order);

        // If buyer selected a specific person, auto-create a delivery assignment request
        if (preference == "SpecificPerson" && preferredDeliveryPersonId.HasValue)
        {
            if (order.SupplierId == null)
                throw new InvalidOperationException("Order must have an assigned supplier before setting a specific delivery person.");

            var deliveryPerson = await _deliveryPersonProfileRepository.GetByIdAsync(preferredDeliveryPersonId.Value, cancellationToken)
                ?? throw new KeyNotFoundException("Selected delivery person not found.");

            var request = new DeliveryAssignmentRequest
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                DeliveryPersonId = preferredDeliveryPersonId.Value,
                SupplierId = order.SupplierId.Value,
                Status = "Pending",
                ProposedFee = deliveryPerson.BaseDeliveryFee
            };
            _deliveryAssignmentRequestRepository.Add(request);

            _notificationRepository.Add(new Notification
            {
                Id = Guid.NewGuid(),
                UserId = deliveryPerson.UserId,
                Type = "DeliveryAssignmentRequest",
                TitleAr = "طلب توصيل جديد",
                TitleEn = "New Delivery Request",
                BodyAr = $"لديك طلب توصيل جديد للطلب '{order.Title}'",
                BodyEn = $"You have a new delivery request for order '{order.Title}'",
                Channel = "InApp",
                RelatedOrderId = orderId
            });
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new { message = "Delivery preference set.", preference = order.DeliveryPreference, preferredId = order.PreferredDeliveryPersonId };
    }

    public async Task<object> ApproveDeliveryFeeAsync(Guid orderId, bool isApproved, string? reason, Guid userId, CancellationToken cancellationToken = default)
    {
        var order = await _groupOrderRepository.GetWithDetailsAsync(orderId, cancellationToken)
            ?? throw new KeyNotFoundException("Order not found.");

        if (order.Creator?.UserId != userId)
            throw new UnauthorizedAccessException("Only the order creator can approve delivery fees.");

        if (order.DeliveryApprovalStatus != "Pending")
            throw new InvalidOperationException("No pending delivery fee to approve.");

        if (isApproved)
        {
            order.DeliveryApprovalStatus = "Approved";
            _groupOrderRepository.Update(order);

            var participantCount = (order.Participants?.Count(p => p.Status == "Joined" && p.Items != null && p.Items.Any(i => i.Quantity > 0)) ?? 0) + 1;
            var feePerPerson = order.ProposedDeliveryFee.GetValueOrDefault() / Math.Max(1, participantCount);

            var invoices = await _invoiceRepository.GetByGroupOrderAsync(order.Id, cancellationToken);
            foreach (var invoice in invoices)
            {
                invoice.DeliveryFee = feePerPerson;
                invoice.Total = invoice.Subtotal + feePerPerson;
                _invoiceRepository.Update(invoice);
            }

            _eventRepository.Add(new GroupOrderEvent
            {
                Id = Guid.NewGuid(),
                GroupOrderId = order.Id,
                EventType = "DeliveryFeeApproved",
                NotesEn = $"Delivery fee approved: {order.ProposedDeliveryFee:C} total, {feePerPerson:C} per participant",
                NotesAr = $"تمت الموافقة على رسوم التوصيل: {order.ProposedDeliveryFee:C} إجمالي، {feePerPerson:C} لكل مشارك",
                CreatedBy = userId
            });
        }
        else
        {
            order.DeliveryApprovalStatus = "Rejected";
            order.ProposedDeliveryFee = null;
            _groupOrderRepository.Update(order);

            _eventRepository.Add(new GroupOrderEvent
            {
                Id = Guid.NewGuid(),
                GroupOrderId = order.Id,
                EventType = "DeliveryFeeRejected",
                NotesEn = $"Delivery fee rejected. Reason: {reason}",
                NotesAr = $"تم رفض رسوم التوصيل. السبب: {reason}",
                CreatedBy = userId
            });
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new { message = isApproved ? "Delivery fee approved." : "Delivery fee rejected.", status = order.DeliveryApprovalStatus };
    }

    public async Task<IReadOnlyList<DeliveryPersonProfileDto>> GetAvailableDeliveryPersonsAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _groupOrderRepository.GetWithDetailsAsync(orderId, cancellationToken)
            ?? throw new KeyNotFoundException("Order not found.");

        if (order.PreferredDeliveryPersonId.HasValue)
        {
            var profile = await _deliveryPersonProfileRepository.GetByIdAsync(order.PreferredDeliveryPersonId.Value, cancellationToken);
            if (profile != null) return [MapToDeliveryPersonDto(profile)];
        }

        var regionId = order.Region?.Id ?? order.RegionId;
        var profiles = await _deliveryPersonProfileRepository.GetForRegionAsync(regionId, cancellationToken);
        return profiles.Select(MapToDeliveryPersonDto).ToList();
    }

    private static DeliveryPersonProfileDto MapToDeliveryPersonDto(DeliveryPersonProfile profile)
    {
        return new DeliveryPersonProfileDto
        {
            Id = profile.Id,
            UserId = profile.UserId,
            FullName = profile.User?.FullName ?? "",
            Email = profile.User?.Email ?? "",
            Phone = profile.User?.Phone,
            VehicleType = profile.VehicleType ?? "",
            BaseDeliveryFee = profile.BaseDeliveryFee,
            Rating = profile.Rating,
            TotalDeliveries = profile.TotalDeliveries,
            IsActive = profile.IsActive,
            CoverageRegionId = profile.CoverageRegionId,
            CoverageRegionName = profile.CoverageRegion?.NameEn
        };
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
}
