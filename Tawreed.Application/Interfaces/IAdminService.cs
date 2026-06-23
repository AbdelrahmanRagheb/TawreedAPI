using Tawreed.Application.Common.Models;
using Tawreed.Domain.Entities;
using Category = Tawreed.Domain.Entities.Category;
using Region = Tawreed.Domain.Entities.Region;

namespace Tawreed.Application.Interfaces;

public class AdminDashboardData
{
    public required AdminKpiDto Kpi { get; set; }
    public required IReadOnlyList<PendingSupplierApplicationDto> PendingSupplierApplications { get; set; }
    public required IReadOnlyList<AdminRecentOrderDto> RecentOrders { get; set; }
}

public class AdminKpiDto
{
    public int TotalUsers { get; set; }
    public int TotalSuppliers { get; set; }
    public int TotalBuyers { get; set; }
    public int TotalOrders { get; set; }
    public int PendingSuppliers { get; set; }
    public int ActiveCategories { get; set; }
    public int NewUsersThisMonth { get; set; }
}

public class PendingSupplierApplicationDto
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public DateTimeOffset SubmittedAt { get; set; }
}

public class AdminRecentOrderDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string BuyerName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}

public class AdminUserListDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? BusinessName { get; set; }
    public string? Region { get; set; }
    public DateTimeOffset JoinedDate { get; set; }
    public DateTimeOffset? LastLoginAt { get; set; }
}

public class AdminUserDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? BusinessName { get; set; }
    public string? BusinessType { get; set; }
    public string? TaxId { get; set; }
    public string? Address { get; set; }
    public string? Region { get; set; }
    public decimal RatingAvg { get; set; }
    public DateTimeOffset JoinedDate { get; set; }
    public DateTimeOffset? LastLoginAt { get; set; }
    public bool EmailVerified { get; set; }
    public bool PhoneVerified { get; set; }
    public string? SuspensionReason { get; set; }
    public int OrdersCreated { get; set; }
    public int OrdersJoined { get; set; }
    public int CompletedOrders { get; set; }
    public int CancelledOrders { get; set; }
    public required List<BuyerOrderItemDto> RecentOrders { get; set; }
}

public class BuyerOrderItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal EstimatedTotal { get; set; }
    public int ParticipantsCount { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class AdminSupplierListDto
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public DateTimeOffset JoinedDate { get; set; }
    public decimal RatingAvg { get; set; }
    public int TotalProducts { get; set; }
    public bool IsApproved { get; set; }
}

public class AdminSupplierDetailDto
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public List<string> CategoryNames { get; set; } = [];
    public string Region { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public DateTimeOffset JoinedDate { get; set; }
    public decimal RatingAvg { get; set; }
    public int TotalProducts { get; set; }
    public string? Address { get; set; }
    public bool EmailVerified { get; set; }
    public bool PhoneVerified { get; set; }
    public DateTimeOffset? LastLoginAt { get; set; }
    public string? SuspensionReason { get; set; }
    public int TotalOrders { get; set; }
    public int ActiveOrders { get; set; }
    public required List<ApprovalLogEntry> ApprovalLogs { get; set; }
    public required List<AdminSupplierProductDto> Products { get; set; }
    public required List<AdminSupplierOrderItemDto> RecentOrders { get; set; }
}

public class AdminSupplierProductDto
{
    public string Name { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int Stock { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public required List<AdminPricingTierDto> PricingTiers { get; set; }
}

public class AdminPricingTierDto
{
    public int MinQty { get; set; }
    public int? MaxQty { get; set; }
    public decimal UnitPrice { get; set; }
}

public class AdminSupplierOrderItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string BuyerName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class AdminOrderListDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string BuyerName { get; set; } = string.Empty;
    public string? BuyerCompany { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string RegionEn { get; set; } = string.Empty;
    public string RegionAr { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset Deadline { get; set; }
    public int Participants { get; set; }
}

public class AdminCategoryListDto
{
    public Guid Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public int ProductCount { get; set; }
    public int SupplierCount { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
}

public class AdminProfileDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string PreferredLang { get; set; } = string.Empty;
    public bool EmailVerified { get; set; }
    public bool PhoneVerified { get; set; }
    public DateTimeOffset? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public record UpdateAdminProfileRequest(
    string? FullName = null,
    string? Phone = null,
    string? AvatarUrl = null,
    string? PreferredLang = null
);

public interface IAdminService
{
    Task<AdminDashboardData> GetDashboardAsync(CancellationToken cancellationToken = default);
    Task<PaginatedResult<AdminUserListDto>> GetUsersAsync(string? search = null, string? status = null, int page = 1, int limit = 20, CancellationToken cancellationToken = default);
    Task<AdminUserDetailDto> GetUserDetailAsync(Guid userId, CancellationToken cancellationToken = default);
    Task SuspendUserAsync(Guid userId, string reason, CancellationToken cancellationToken = default);
    Task ReactivateUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<string> ResetUserPasswordAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<PaginatedResult<AdminSupplierListDto>> GetSuppliersAsync(string? search = null, string? status = null, Guid? categoryId = null, Guid? regionId = null, int page = 1, int limit = 20, CancellationToken cancellationToken = default);
    Task<AdminSupplierDetailDto> GetSupplierDetailAsync(Guid supplierId, CancellationToken cancellationToken = default);
    Task ApproveSupplierAsync(Guid supplierId, Guid adminUserId, CancellationToken cancellationToken = default);
    Task RejectSupplierAsync(Guid supplierId, Guid adminUserId, string reason, CancellationToken cancellationToken = default);
    Task SuspendSupplierAsync(Guid supplierId, Guid adminUserId, string reason, CancellationToken cancellationToken = default);
    Task ReactivateSupplierAsync(Guid supplierId, Guid adminUserId, CancellationToken cancellationToken = default);
    Task<PaginatedResult<AdminOrderListDto>> GetOrdersAsync(string? status = null, Guid? regionId = null, int page = 1, int limit = 20, CancellationToken cancellationToken = default);
    Task<object> GetOrderDetailAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task ForceCloseOrderAsync(Guid orderId, Guid adminUserId, string reason, CancellationToken cancellationToken = default);
    Task<PaginatedResult<AdminCategoryListDto>> GetCategoriesAsync(string? search = null, int page = 1, int limit = 20, CancellationToken cancellationToken = default);
    Task<object> GetCategoryDetailAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<CategoryDto> CreateCategoryAsync(string nameAr, string nameEn, Guid? parentId, string? iconUrl, int sortOrder, CancellationToken cancellationToken = default);
    Task UpdateCategoryAsync(Guid categoryId, string? nameAr, string? nameEn, Guid? parentId, string? iconUrl, int? sortOrder, bool? isActive, CancellationToken cancellationToken = default);
    Task DeactivateCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task ActivateCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task SoftDeleteCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<List<RegionDto>> GetRegionsAsync(string? search = null, CancellationToken cancellationToken = default);
    Task<RegionDto> CreateRegionAsync(string nameAr, string nameEn, Guid? parentId, string type, CancellationToken cancellationToken = default);
    Task ToggleRegionAsync(Guid regionId, CancellationToken cancellationToken = default);
    Task<List<RegionTreeNodeDto>> GetRegionTreeAsync(CancellationToken cancellationToken = default);
    Task<RegionStatsDto> GetRegionStatsAsync(CancellationToken cancellationToken = default);
    Task<List<RegionTreeNodeDto>> GetRegionRootsAsync(CancellationToken cancellationToken = default);
    Task<List<RegionTreeNodeDto>> GetRegionChildrenAsync(Guid parentId, CancellationToken cancellationToken = default);
    Task<RegionTreeNodeDto> UpdateRegionAsync(Guid regionId, string nameAr, string nameEn, Guid? parentId, string? type, CancellationToken cancellationToken = default);
    Task DeleteRegionAsync(Guid regionId, CancellationToken cancellationToken = default);

    Task<AdminProfileDto> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default);
    Task UpdateProfileAsync(Guid userId, UpdateAdminProfileRequest request, CancellationToken cancellationToken = default);

    Task<List<string>> GetGroupRegionTypesAsync(CancellationToken cancellationToken = default);
    Task SetGroupRegionTypesAsync(List<string> types, CancellationToken cancellationToken = default);
}
