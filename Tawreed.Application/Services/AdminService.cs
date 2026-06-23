using System.Text.Json;
using Tawreed.Application.Common.Models;
using Tawreed.Application.Interfaces;
using Tawreed.Domain.Entities;
using Tawreed.Domain.Enums;
using Tawreed.Domain.Interfaces;
using Category = Tawreed.Domain.Entities.Category;
using Region = Tawreed.Domain.Entities.Region;

namespace Tawreed.Application.Services;

public class AdminService : IAdminService
{
    private readonly IUserRepository _userRepository;
    private readonly ISupplierRepository _supplierRepository;
    private readonly IBuyerRepository _buyerRepository;
    private readonly IGroupOrderRepository _groupOrderRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IRegionRepository _regionRepository;
    private readonly ISupplierApprovalLogRepository _approvalLogRepository;
    private readonly ISupplierCategoryRepository _supplierCategoryRepository;
    private readonly ISupplierProductRepository _supplierProductRepository;
    private readonly IGroupOrderParticipantRepository _participantRepository;
    private readonly IGroupOrderEventRepository _eventRepository;
    private readonly IPricingTierRepository _pricingTierRepository;
    private readonly IAppSettingRepository _appSettingRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AdminService(
        IUserRepository userRepository,
        ISupplierRepository supplierRepository,
        IBuyerRepository buyerRepository,
        IGroupOrderRepository groupOrderRepository,
        ICategoryRepository categoryRepository,
        IRegionRepository regionRepository,
        ISupplierApprovalLogRepository approvalLogRepository,
        ISupplierCategoryRepository supplierCategoryRepository,
        ISupplierProductRepository supplierProductRepository,
        IGroupOrderParticipantRepository participantRepository,
        IGroupOrderEventRepository eventRepository,
        IPricingTierRepository pricingTierRepository,
        IAppSettingRepository appSettingRepository,
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _supplierRepository = supplierRepository;
        _buyerRepository = buyerRepository;
        _groupOrderRepository = groupOrderRepository;
        _categoryRepository = categoryRepository;
        _regionRepository = regionRepository;
        _approvalLogRepository = approvalLogRepository;
        _supplierCategoryRepository = supplierCategoryRepository;
        _supplierProductRepository = supplierProductRepository;
        _participantRepository = participantRepository;
        _eventRepository = eventRepository;
        _pricingTierRepository = pricingTierRepository;
        _appSettingRepository = appSettingRepository;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<AdminDashboardData> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var allUsers = await _userRepository.GetAllAsync(cancellationToken);
        var allSuppliers = await _supplierRepository.GetAllAsync(cancellationToken);
        var allBuyers = await _buyerRepository.GetAllAsync(cancellationToken);
        var allOrders = await _groupOrderRepository.GetAllAsync(cancellationToken);
        var allCategories = await _categoryRepository.GetAllAsync(cancellationToken);

        var now = DateTimeOffset.UtcNow;
        var startOfMonth = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, now.Offset);

        return new AdminDashboardData
        {
            Kpi = new AdminKpiDto
            {
                TotalUsers = allUsers.Count,
                TotalSuppliers = allSuppliers.Count,
                TotalBuyers = allBuyers.Count,
                TotalOrders = allOrders.Count,
                PendingSuppliers = allSuppliers.Count(s => s.User != null && s.User.Status == "PendingApproval"),
                ActiveCategories = allCategories.Count(c => c.IsActive && !c.IsDeleted),
                NewUsersThisMonth = allUsers.Count(u => u.CreatedAt >= startOfMonth)
            },
PendingSupplierApplications = allSuppliers
    .Where(s => s.User != null && s.User.Status == "PendingApproval")
    .Select(s => new PendingSupplierApplicationDto
                {
                    Id = s.Id,
                    CompanyName = s.CompanyName,
                    ContactName = s.User?.FullName ?? "",
                    Email = s.User?.Email ?? "",
                    Category = s.SupplierCategories?.FirstOrDefault()?.Category?.NameEn ?? "",
                    Region = s.Region?.NameEn ?? "",
                    SubmittedAt = s.CreatedAt
                })
                .OrderBy(s => s.SubmittedAt)
                .Take(10)
                .ToList(),
            RecentOrders = allOrders
                .OrderByDescending(o => o.CreatedAt)
                .Take(10)
                .Select(o => new AdminRecentOrderDto
                {
                    Id = o.Id,
                    Title = o.Title,
                    BuyerName = o.Creator?.User?.FullName ?? "",
                    TotalAmount = o.Items?.Sum(i => (i.UnitPrice ?? 0) * i.TargetQty) ?? 0,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt
                })
                .ToList()
        };
    }

    public async Task<PaginatedResult<AdminUserListDto>> GetUsersAsync(string? search = null, string? status = null, int page = 1, int limit = 20, CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        var buyers = await _buyerRepository.GetAllAsync(cancellationToken);
        var buyerMap = buyers.ToDictionary(b => b.UserId);

        var filtered = users.Where(u => u.Role == "Buyer").AsEnumerable();

        if (!string.IsNullOrEmpty(search))
            filtered = filtered.Where(u => u.FullName.Contains(search) || u.Email.Contains(search));
        if (!string.IsNullOrEmpty(status))
            filtered = filtered.Where(u => u.Status == status);

        var list = filtered.ToList();
        var total = list.Count;
        var paged = list.Skip((page - 1) * limit).Take(limit).ToList();

        return new PaginatedResult<AdminUserListDto>
        {
            Items = paged.Select(u =>
            {
                buyerMap.TryGetValue(u.Id, out var b);
                return new AdminUserListDto
                {
                    Id = u.Id, Name = u.FullName, Email = u.Email, Phone = u.Phone,
                    Role = u.Role, Status = u.Status, BusinessName = b?.BusinessName,
                    Region = b?.Region?.NameEn, JoinedDate = u.CreatedAt, LastLoginAt = u.LastLoginAt
                };
            }).ToList(),
            Page = page, Limit = limit, Total = total,
            TotalPages = (int)Math.Ceiling((double)total / limit)
        };
    }

    public async Task<AdminUserDetailDto> GetUserDetailAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("User not found.");

        var buyers = await _buyerRepository.GetAllAsync(cancellationToken);
        var buyer = buyers.FirstOrDefault(b => b.UserId == userId);

        var allOrders = await _groupOrderRepository.GetAllAsync(cancellationToken);
        var createdOrders = buyer != null
            ? allOrders.Where(o => o.CreatorId == buyer.Id).ToList()
            : [];

        var ordersJoined = 0;
        if (buyer != null)
        {
            var allParticipants = await _participantRepository.GetAllAsync(cancellationToken);
            ordersJoined = allParticipants.Count(p => p.BuyerId == buyer.Id && p.Status == "Joined");
        }

        var recentOrders = createdOrders
            .OrderByDescending(o => o.CreatedAt)
            .Take(10)
            .Select(o => new BuyerOrderItemDto
            {
                Id = o.Id,
                Title = o.Title,
                Status = o.Status,
                EstimatedTotal = o.Items?.Sum(i => (i.UnitPrice ?? 0) * i.TargetQty) ?? 0,
                ParticipantsCount = o.Participants?.Count(p => p.Status == "Joined") ?? 0,
                CreatedAt = o.CreatedAt
            })
            .ToList();

        return new AdminUserDetailDto
        {
            Id = user.Id,
            Name = user.FullName,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role,
            Status = user.Status,
            BusinessName = buyer?.BusinessName,
            BusinessType = buyer?.BusinessType,
            TaxId = buyer?.TaxId,
            Address = buyer?.Address,
            Region = buyer?.Region?.NameEn,
            RatingAvg = buyer?.RatingAvg ?? 0,
            JoinedDate = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            EmailVerified = user.EmailVerified,
            PhoneVerified = user.PhoneVerified,
            SuspensionReason = null,
            OrdersCreated = createdOrders.Count,
            OrdersJoined = ordersJoined,
            CompletedOrders = createdOrders.Count(o => o.Status == OrderStatus.Completed),
            CancelledOrders = createdOrders.Count(o => o.Status == OrderStatus.Cancelled),
            RecentOrders = recentOrders
        };
    }

    public async Task SuspendUserAsync(Guid userId, string reason, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("User not found.");
        user.Status = "Suspended";
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ReactivateUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("User not found.");
        user.Status = "Active";
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<string> ResetUserPasswordAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("User not found.");
        var tempPassword = "Temp@" + Guid.NewGuid().ToString("N")[..8];
        user.PasswordHash = tempPassword;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return tempPassword;
    }

    public async Task<PaginatedResult<AdminSupplierListDto>> GetSuppliersAsync(string? search = null, string? status = null, Guid? categoryId = null, Guid? regionId = null, int page = 1, int limit = 20, CancellationToken cancellationToken = default)
    {
        var suppliers = await _supplierRepository.GetAllAsync(cancellationToken);
        var filtered = suppliers.AsEnumerable();

        if (!string.IsNullOrEmpty(search))
            filtered = filtered.Where(s => s.CompanyName.Contains(search) || (s.User?.FullName?.Contains(search) ?? false) || (s.User?.Email?.Contains(search) ?? false));
        if (!string.IsNullOrEmpty(status) && status != "all")
            filtered = filtered.Where(s => s.User != null && s.User.Status == status);
        if (categoryId.HasValue)
            filtered = filtered.Where(s => s.SupplierCategories?.Any(sc => sc.CategoryId == categoryId.Value) == true);
        if (regionId.HasValue)
            filtered = filtered.Where(s => s.RegionId == regionId.Value);

        var list = filtered.ToList();
        var total = list.Count;
        var paged = list.Skip((page - 1) * limit).Take(limit).ToList();

        return new PaginatedResult<AdminSupplierListDto>
        {
            Items = paged.Select(s => new AdminSupplierListDto
            {
                Id = s.Id, CompanyName = s.CompanyName,
                ContactName = s.User?.FullName ?? "", Email = s.User?.Email ?? "",
                Phone = s.User?.Phone, Category = s.SupplierCategories?.FirstOrDefault()?.Category?.NameEn ?? "",
                Status = s.User?.Status ?? "", Region = s.Region?.NameEn ?? "",
                JoinedDate = s.CreatedAt, RatingAvg = s.RatingAvg,
                TotalProducts = s.SupplierProducts?.Count ?? 0, IsApproved = s.IsApproved
            }).ToList(),
            Page = page, Limit = limit, Total = total,
            TotalPages = (int)Math.Ceiling((double)total / limit)
        };
    }

    public async Task<AdminSupplierDetailDto> GetSupplierDetailAsync(Guid supplierId, CancellationToken cancellationToken = default)
    {
        var suppliers = await _supplierRepository.GetAllAsync(cancellationToken);
        var supplier = suppliers.FirstOrDefault(s => s.Id == supplierId)
            ?? throw new KeyNotFoundException("Supplier not found.");

        var logs = await _approvalLogRepository.GetBySupplierAsync(supplierId, cancellationToken);
        var products = await _supplierProductRepository.GetBySupplierAsync(supplierId, cancellationToken);

        var supplierOrders = await _groupOrderRepository.GetBySupplierAsync(supplierId, cancellationToken);

        var allTiers = await _pricingTierRepository.GetAllAsync(cancellationToken);
        var productIds = products.Select(p => p.Id).ToHashSet();
        var tierMap = allTiers
            .Where(t => productIds.Contains(t.SupplierProductId))
            .GroupBy(t => t.SupplierProductId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var suspensionLog = logs.FirstOrDefault(l => l.Action == "Suspended");

        return new AdminSupplierDetailDto
        {
            Id = supplier.Id, CompanyName = supplier.CompanyName,
            ContactName = supplier.User?.FullName ?? "", Email = supplier.User?.Email ?? "",
            Phone = supplier.User?.Phone,
            CategoryNames = supplier.SupplierCategories?.Select(sc => sc.Category?.NameEn ?? "").ToList() ?? [],
            Region = supplier.Region?.NameEn ?? "", Status = supplier.User?.Status ?? "",
            IsApproved = supplier.IsApproved, JoinedDate = supplier.CreatedAt,
            RatingAvg = supplier.RatingAvg, TotalProducts = products.Count,
            Address = supplier.Address,
            EmailVerified = supplier.User?.EmailVerified ?? false,
            PhoneVerified = supplier.User?.PhoneVerified ?? false,
            LastLoginAt = supplier.User?.LastLoginAt,
            SuspensionReason = suspensionLog?.Reason,
            TotalOrders = supplierOrders.Count,
            ActiveOrders = supplierOrders.Count(o => o.Status == OrderStatus.Open),
            ApprovalLogs = logs.Select(l => new ApprovalLogEntry
            {
                Action = l.Action, Reason = l.Reason,
                ActorName = l.Actor?.FullName ?? "", CreatedAt = l.CreatedAt
            }).OrderByDescending(l => l.CreatedAt).ToList(),
            Products = products.Select(p => new AdminSupplierProductDto
            {
                Name = p.Product?.Name ?? "", CategoryName = p.Product?.Category?.NameEn ?? "",
                Stock = p.Stock, Unit = p.Product?.Unit?.Symbol ?? "",
                Price = p.Price,
                PricingTiers = (tierMap.TryGetValue(p.Id, out var tiers)
                    ? tiers.Select(t => new AdminPricingTierDto
                    {
                        MinQty = t.MinQty, MaxQty = t.MaxQty, UnitPrice = t.UnitPrice
                    }).OrderBy(t => t.MinQty).ToList()
                    : [])
            }).ToList(),
            RecentOrders = supplierOrders
                .OrderByDescending(o => o.CreatedAt)
                .Take(10)
                .Select(o => new AdminSupplierOrderItemDto
                {
                    Id = o.Id, Title = o.Title,
                    BuyerName = o.Creator?.User?.FullName ?? "",
                    Status = o.Status,
                    TotalAmount = o.Items?.Sum(i => (i.UnitPrice ?? 0) * i.TargetQty) ?? 0,
                    CreatedAt = o.CreatedAt
                }).ToList()
        };
    }

    public async Task ApproveSupplierAsync(Guid supplierId, Guid adminUserId, CancellationToken cancellationToken = default)
    {
        await UpdateSupplierStatus(supplierId, adminUserId, "Approved", null, cancellationToken);
    }

    public async Task RejectSupplierAsync(Guid supplierId, Guid adminUserId, string reason, CancellationToken cancellationToken = default)
    {
        await UpdateSupplierStatus(supplierId, adminUserId, "Rejected", reason, cancellationToken);
    }

    public async Task SuspendSupplierAsync(Guid supplierId, Guid adminUserId, string reason, CancellationToken cancellationToken = default)
    {
        await UpdateSupplierStatus(supplierId, adminUserId, "Suspended", reason, cancellationToken);
    }

    public async Task ReactivateSupplierAsync(Guid supplierId, Guid adminUserId, CancellationToken cancellationToken = default)
    {
        await UpdateSupplierStatus(supplierId, adminUserId, "Reactivated", null, cancellationToken);
    }

    private async Task UpdateSupplierStatus(Guid supplierId, Guid adminUserId, string action, string? reason, CancellationToken cancellationToken)
    {
        var suppliers = await _supplierRepository.GetAllAsync(cancellationToken);
        var supplier = suppliers.FirstOrDefault(s => s.Id == supplierId)
            ?? throw new KeyNotFoundException("Supplier not found.");
        var user = supplier.User ?? throw new KeyNotFoundException("User not found.");

        if (action == "Approved" || action == "Reactivated")
        {
            supplier.IsApproved = true;
            supplier.ApprovedBy = adminUserId;
            supplier.ApprovedAt = DateTimeOffset.UtcNow;
            user.Status = "Active";
        }
        else if (action == "Rejected")
        {
            supplier.IsApproved = false;
            user.Status = "Rejected";
        }
        else if (action == "Suspended")
        {
            supplier.IsApproved = false;
            user.Status = "Suspended";
        }

        _approvalLogRepository.Add(new SupplierApprovalLog
        {
            Id = Guid.NewGuid(), SupplierId = supplierId,
            Action = action, ActorId = adminUserId, Reason = reason
        });

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Notify the supplier about their approval status change
        var (titleAr, titleEn, bodyAr, bodyEn) = action switch
        {
            "Approved" => ("تمت موافقتك كمورد", "Supplier Account Approved",
                "تهانينا! تمت موافقتك كمورد على منصة توريد. يمكنك الآن استقبال الطلبات.",
                "Congratulations! Your supplier account on Tawreed has been approved. You can now receive orders."),
            "Reactivated" => ("تمت إعادة تفعيل حسابك", "Supplier Account Reactivated",
                "تمت إعادة تفعيل حسابك بنجاح. مرحبًا بك مرة أخرى!",
                "Your account has been reactivated successfully. Welcome back!"),
            "Rejected" => ("تمت رفض طلبك", "Supplier Application Rejected",
                $"نعتذر، تم رفض طلب انتسابك كمورد. السبب: {reason ?? "لم يحدد"}.",
                $"Unfortunately, your supplier application has been rejected. Reason: {reason ?? "Not specified"}."),
            "Suspended" => ("تم تعليق حسابك", "Supplier Account Suspended",
                $"تم تعليق حسابك على منصة توريد. السبب: {reason ?? "لم يحدد"}.",
                $"Your supplier account on Tawreed has been suspended. Reason: {reason ?? "Not specified"}."),
            _ => ("تحديث حالة الحساب", "Account Status Updated",
                "تم تحديث حالة حسابك.",
                "Your account status has been updated.")
        };

        _notificationRepository.Add(new Notification
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Type = "SupplierStatusChanged",
            TitleAr = titleAr,
            TitleEn = titleEn,
            BodyAr = bodyAr,
            BodyEn = bodyEn,
            Channel = "InApp"
        });

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<PaginatedResult<AdminOrderListDto>> GetOrdersAsync(string? status = null, Guid? regionId = null, int page = 1, int limit = 20, CancellationToken cancellationToken = default)
    {
        var orders = await _groupOrderRepository.GetAllWithAdminDetailsAsync(cancellationToken);
        var filtered = orders.AsEnumerable();

        if (!string.IsNullOrEmpty(status))
            filtered = filtered.Where(o => o.Status == status);
        if (regionId.HasValue)
            filtered = filtered.Where(o => o.RegionId == regionId.Value);

        var list = filtered.ToList();
        var total = list.Count;
        var paged = list.Skip((page - 1) * limit).Take(limit).ToList();

        return new PaginatedResult<AdminOrderListDto>
        {
            Items = paged.Select(o => new AdminOrderListDto
            {
                Id = o.Id, Title = o.Title, BuyerName = o.Creator?.User?.FullName ?? "",
                BuyerCompany = o.Creator?.BusinessName, SupplierName = o.Supplier?.CompanyName ?? "",
                TotalAmount = o.Items?.Sum(i => (i.UnitPrice ?? 0) * i.TargetQty) ?? 0,
                Status = o.Status,
                RegionEn = o.Region?.NameEn ?? "",
                RegionAr = o.Region?.NameAr ?? "",
                CreatedAt = o.CreatedAt, Deadline = o.DeadlineAt,
                Participants = o.Participants?.Count(p => p.Status == "Joined") ?? 0
            }).ToList(),
            Page = page, Limit = limit, Total = total,
            TotalPages = (int)Math.Ceiling((double)total / limit)
        };
    }

    public async Task<object> GetOrderDetailAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _groupOrderRepository.GetWithDetailsAsync(orderId, cancellationToken)
            ?? throw new KeyNotFoundException("Order not found.");

        return new
        {
            id = order.Id.ToString(),
            title = order.Title,
            buyer = new
            {
                id = order.Creator?.UserId.ToString(),
                name = order.Creator?.User?.FullName ?? "",
                company = order.Creator?.BusinessName,
                email = order.Creator?.User?.Email ?? "",
                phone = order.Creator?.User?.Phone ?? ""
            },
            supplier = order.SupplierId.HasValue ? new
            {
                id = order.SupplierId.ToString(),
                name = order.Supplier?.User?.FullName ?? "",
                companyName = order.Supplier?.CompanyName ?? ""
            } : null,
            regionEn = order.Region?.NameEn ?? "",
            regionAr = order.Region?.NameAr ?? "",
            createdAt = order.CreatedAt,
            deadline = order.DeadlineAt,
            status = order.Status,
            totalOrderValue = order.Items?.Sum(i => (i.UnitPrice ?? 0) * i.TargetQty) ?? 0,
            items = order.Items?.Select(i => new
            {
                productId = i.ProductId.ToString(),
                productName = i.Product?.Name ?? "",
                quantity = i.ParticipantItems?.Sum(pi => pi.Quantity) ?? 0,
                targetQuantity = i.TargetQty,
                unitPrice = i.UnitPrice ?? 0,
                totalPrice = (i.UnitPrice ?? 0) * (i.ParticipantItems?.Sum(pi => pi.Quantity) ?? 0)
            }).ToList() ?? [],
            participants = (order.Participants?.Where(p => p.Status == "Joined") ?? []).Select(p => new
            {
                id = p.Id.ToString(),
                name = p.Buyer?.User?.FullName ?? "",
                joinedAt = p.JoinedAt,
                items = p.Items?.Select(pi => new { productId = pi.GroupOrderItem?.ProductId.ToString(), quantity = pi.Quantity }).ToList()
            }).ToList(),
            timeline = order.Events?.Select(e => new
            {
                eventType = e.EventType,
                notes = e.NotesEn,
                actorName = e.Creator?.FullName ?? "",
                createdAt = e.CreatedAt
            }).OrderByDescending(e => e.createdAt).ToList() ?? []
        };
    }

    public async Task ForceCloseOrderAsync(Guid orderId, Guid adminUserId, string reason, CancellationToken cancellationToken = default)
    {
        var order = await _groupOrderRepository.GetWithDetailsAsync(orderId, cancellationToken)
            ?? throw new KeyNotFoundException("Order not found.");

        order.Status = OrderStatus.Cancelled;
        order.ClosedAt = DateTimeOffset.UtcNow;
        _groupOrderRepository.Update(order);

        _eventRepository.Add(new GroupOrderEvent
        {
            Id = Guid.NewGuid(), GroupOrderId = order.Id,
            EventType = "Cancelled", NotesEn = reason, CreatedBy = adminUserId
        });

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Notify all participants (creator + joined buyers)
        var participantUserIds = GetAllOrderParticipantUserIds(order);
        foreach (var participantUserId in participantUserIds)
        {
            _notificationRepository.Add(new Notification
            {
                Id = Guid.NewGuid(),
                UserId = participantUserId,
                Type = "OrderForceClosed",
                TitleAr = "تم إلغاء الطلب الجماعي",
                TitleEn = "Group Order Has Been Cancelled",
                BodyAr = $"تم إلغاء الطلب الجماعي '{order.Title}' من قبل الإدارة. السبب: {reason}",
                BodyEn = $"The group order '{order.Title}' has been cancelled by administration. Reason: {reason}",
                Channel = "InApp",
                RelatedOrderId = orderId
            });
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static List<Guid> GetAllOrderParticipantUserIds(GroupOrder order)
    {
        var ids = new List<Guid>();
        if (order.Creator?.UserId is Guid creatorId)
            ids.Add(creatorId);

        var creatorBuyerId = order.CreatorId;
        var participantIds = order.Participants?
            .Where(p => p.Status == "Joined" && p.BuyerId != creatorBuyerId && p.Buyer?.UserId != null)
            .Select(p => p.Buyer!.UserId)
            .ToList() ?? [];

        ids.AddRange(participantIds);
        return ids.Distinct().ToList();
    }

    public async Task<PaginatedResult<AdminCategoryListDto>> GetCategoriesAsync(string? search = null, int page = 1, int limit = 20, CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        var filtered = categories.AsEnumerable();

        if (!string.IsNullOrEmpty(search))
            filtered = filtered.Where(c => c.NameEn.Contains(search) || c.NameAr.Contains(search));

        var list = filtered.ToList();
        var total = list.Count;
        var paged = list.Skip((page - 1) * limit).Take(limit).ToList();

        return new PaginatedResult<AdminCategoryListDto>
        {
            Items = paged.Select(c => new AdminCategoryListDto
            {
                Id = c.Id, NameAr = c.NameAr, NameEn = c.NameEn,
                ProductCount = c.Products?.Count ?? 0,
                SupplierCount = c.SupplierCategories?.Count ?? 0,
                IsActive = c.IsActive, SortOrder = c.SortOrder
            }).ToList(),
            Page = page, Limit = limit, Total = total,
            TotalPages = (int)Math.Ceiling((double)total / limit)
        };
    }

    public async Task<object> GetCategoryDetailAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken)
            ?? throw new KeyNotFoundException("Category not found.");

        return new
        {
            id = category.Id.ToString(),
            nameAr = category.NameAr,
            nameEn = category.NameEn,
            productCount = category.Products?.Count ?? 0,
            supplierCount = category.SupplierCategories?.Count ?? 0,
            isActive = category.IsActive,
            sortOrder = category.SortOrder,
            parentId = category.ParentId?.ToString(),
            iconUrl = category.IconUrl
        };
    }

    public async Task<CategoryDto> CreateCategoryAsync(string nameAr, string nameEn, Guid? parentId, string? iconUrl, int sortOrder, CancellationToken cancellationToken = default)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(), NameAr = nameAr, NameEn = nameEn,
            ParentId = parentId, IconUrl = iconUrl, SortOrder = sortOrder,
            IsActive = true
        };
        _categoryRepository.Add(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new CategoryDto(category.Id, category.NameAr, category.NameEn, category.ParentId, category.IconUrl, category.SortOrder, category.IsActive);
    }

    public async Task UpdateCategoryAsync(Guid categoryId, string? nameAr, string? nameEn, Guid? parentId, string? iconUrl, int? sortOrder, bool? isActive, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken)
            ?? throw new KeyNotFoundException("Category not found.");

        if (nameAr != null) category.NameAr = nameAr;
        if (nameEn != null) category.NameEn = nameEn;
        if (parentId.HasValue) category.ParentId = parentId;
        if (iconUrl != null) category.IconUrl = iconUrl;
        if (sortOrder.HasValue) category.SortOrder = sortOrder.Value;
        if (isActive.HasValue) category.IsActive = isActive.Value;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken)
            ?? throw new KeyNotFoundException("Category not found.");
        category.IsActive = false;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ActivateCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken)
            ?? throw new KeyNotFoundException("Category not found.");
        category.IsActive = true;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task SoftDeleteCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken)
            ?? throw new KeyNotFoundException("Category not found.");
        _categoryRepository.Delete(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<RegionDto>> GetRegionsAsync(string? search = null, CancellationToken cancellationToken = default)
    {
        var regions = await _regionRepository.GetActiveAsync(cancellationToken);
        if (!string.IsNullOrEmpty(search))
            regions = regions.Where(r => r.NameEn.Contains(search) || r.NameAr.Contains(search)).ToList();
        return regions.Select(r => new RegionDto(r.Id, r.NameAr, r.NameEn, r.ParentId, r.Type, r.IsActive, r.CreatedAt, r.UpdatedAt)).ToList();
    }

    public async Task<RegionDto> CreateRegionAsync(string nameAr, string nameEn, Guid? parentId, string type, CancellationToken cancellationToken = default)
    {
        var regionType = Enum.Parse<RegionType>(type);
        var region = new Region
        {
            Id = Guid.NewGuid(), NameAr = nameAr, NameEn = nameEn,
            ParentId = parentId, Type = regionType, IsActive = true,
        };
        _regionRepository.Add(region);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new RegionDto(region.Id, region.NameAr, region.NameEn, region.ParentId, region.Type, region.IsActive, region.CreatedAt, region.UpdatedAt);
    }

    public async Task ToggleRegionAsync(Guid regionId, CancellationToken cancellationToken = default)
    {
        var region = await _regionRepository.GetByIdAsync(regionId, cancellationToken)
            ?? throw new KeyNotFoundException("Region not found.");
        region.IsActive = !region.IsActive;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<RegionTreeNodeDto>> GetRegionTreeAsync(CancellationToken cancellationToken = default)
    {
        var all = await _regionRepository.GetAllAsync(cancellationToken);
        var lookup = all.ToDictionary(r => r.Id);
        var roots = all.Where(r => r.ParentId == null).OrderBy(r => r.NameEn).ToList();

        return roots.Select(r => BuildTreeNode(r, all, lookup)).ToList();
    }

    public async Task<RegionTreeNodeDto> UpdateRegionAsync(Guid regionId, string nameAr, string nameEn, Guid? parentId, string? type, CancellationToken cancellationToken = default)
    {
        var region = await _regionRepository.GetByIdAsync(regionId, cancellationToken)
            ?? throw new KeyNotFoundException("Region not found.");
        region.NameAr = nameAr;
        region.NameEn = nameEn;
        region.ParentId = parentId;
        if (type is not null)
            region.Type = Enum.Parse<RegionType>(type);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new RegionTreeNodeDto
        {
            Id = region.Id, NameAr = region.NameAr, NameEn = region.NameEn,
            ParentId = region.ParentId, Type = region.Type, IsActive = region.IsActive,
            CreatedAt = region.CreatedAt, UpdatedAt = region.UpdatedAt,
        };
    }

    public async Task DeleteRegionAsync(Guid regionId, CancellationToken cancellationToken = default)
    {
        var region = await _regionRepository.GetByIdAsync(regionId, cancellationToken)
            ?? throw new KeyNotFoundException("Region not found.");
        var descendants = await _regionRepository.GetDescendantIdsAsync(regionId, cancellationToken);
        var idsToDelete = new List<Guid> { regionId };
        idsToDelete.AddRange(descendants);

        foreach (var id in idsToDelete)
        {
            var r = await _regionRepository.GetByIdAsync(id, cancellationToken);
            if (r != null)
                _regionRepository.Delete(r);
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<RegionStatsDto> GetRegionStatsAsync(CancellationToken cancellationToken = default)
    {
        var governorates = await _regionRepository.CountAsync(r => r.Type == RegionType.Governorate, cancellationToken);
        var cities = await _regionRepository.CountAsync(r =>
            r.Type == RegionType.Markaz ||
            r.Type == RegionType.Qism ||
            r.Type == RegionType.Madina ||
            r.Type == RegionType.Hayy ||
            r.Type == RegionType.PoliceDepartment ||
            r.Type == RegionType.Region ||
            r.Type == RegionType.City, cancellationToken);
        var villages = await _regionRepository.CountAsync(r =>
            r.Type == RegionType.Village ||
            r.Type == RegionType.Kafr ||
            r.Type == RegionType.Ezba ||
            r.Type == RegionType.Shiyakha ||
            r.Type == RegionType.Manshaat ||
            r.Type == RegionType.Zone ||
            r.Type == RegionType.CustomsZone ||
            r.Type == RegionType.QismSection, cancellationToken);
        return new RegionStatsDto(governorates, cities, villages);
    }

    public async Task<List<RegionTreeNodeDto>> GetRegionRootsAsync(CancellationToken cancellationToken = default)
    {
        var egypt = await _regionRepository.FindOneAsync(r => r.ParentId == null, cancellationToken);
        if (egypt is null) return [];

        var governorates = await _regionRepository.FindAsync(r => r.ParentId == egypt.Id, cancellationToken);
        return governorates.OrderBy(r => r.NameEn).Select(r => MapToTreeNode(r, null)).ToList();
    }

    public async Task<List<RegionTreeNodeDto>> GetRegionChildrenAsync(Guid parentId, CancellationToken cancellationToken = default)
    {
        var parent = await _regionRepository.GetByIdAsync(parentId, cancellationToken);
        if (parent is null) return [];

        var children = await _regionRepository.FindAsync(r => r.ParentId == parentId, cancellationToken);
        return children.OrderBy(r => r.NameEn).Select(r => MapToTreeNode(r, parent.NameEn)).ToList();
    }

    private static RegionTreeNodeDto MapToTreeNode(Region region, string? parentName)
    {
        return new RegionTreeNodeDto
        {
            Id = region.Id, NameAr = region.NameAr, NameEn = region.NameEn,
            ParentId = region.ParentId, Type = region.Type, IsActive = region.IsActive,
            CreatedAt = region.CreatedAt, UpdatedAt = region.UpdatedAt,
            ParentName = parentName,
        };
    }

    private static RegionTreeNodeDto BuildTreeNode(Region region, IEnumerable<Region> all, Dictionary<Guid, Region> lookup)
    {
        var node = new RegionTreeNodeDto
        {
            Id = region.Id, NameAr = region.NameAr, NameEn = region.NameEn,
            ParentId = region.ParentId, Type = region.Type, IsActive = region.IsActive,
            CreatedAt = region.CreatedAt, UpdatedAt = region.UpdatedAt,
            ParentName = region.ParentId.HasValue && lookup.TryGetValue(region.ParentId.Value, out var parent)
                ? parent.NameEn : null,
        };
        var children = all.Where(r => r.ParentId == region.Id).OrderBy(r => r.NameEn).ToList();
        node.Children = children.Select(c => BuildTreeNode(c, all, lookup)).ToList();
        return node;
    }

    public async Task<AdminProfileDto> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("Admin user not found.");

        return new AdminProfileDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role,
            Status = user.Status,
            AvatarUrl = user.AvatarUrl,
            PreferredLang = user.PreferredLang,
            EmailVerified = user.EmailVerified,
            PhoneVerified = user.PhoneVerified,
            LastLoginAt = user.LastLoginAt,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task UpdateProfileAsync(Guid userId, UpdateAdminProfileRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("Admin user not found.");

        if (request.FullName != null) user.FullName = request.FullName;
        if (request.Phone != null) user.Phone = request.Phone;
        if (request.AvatarUrl != null) user.AvatarUrl = request.AvatarUrl;
        if (request.PreferredLang != null) user.PreferredLang = request.PreferredLang;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<string>> GetGroupRegionTypesAsync(CancellationToken cancellationToken = default)
    {
        var setting = await _appSettingRepository.GetByKeyAsync("GroupRegionTypes", cancellationToken);
        if (setting is null || string.IsNullOrWhiteSpace(setting.Value))
            return [];

        try
        {
            return JsonSerializer.Deserialize<List<string>>(setting.Value) ?? [];
        }
        catch
        {
            return [];
        }
    }

    public async Task SetGroupRegionTypesAsync(List<string> types, CancellationToken cancellationToken = default)
    {
        var setting = await _appSettingRepository.GetByKeyAsync("GroupRegionTypes", cancellationToken);
        var json = JsonSerializer.Serialize(types);

        if (setting is null)
        {
            _appSettingRepository.Add(new AppSetting
            {
                Id = Guid.NewGuid(),
                Key = "GroupRegionTypes",
                Value = json,
                Description = "Region types that can host group orders"
            });
        }
        else
        {
            setting.Value = json;
            _appSettingRepository.Update(setting);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
