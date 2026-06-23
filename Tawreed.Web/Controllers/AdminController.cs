using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tawreed.Application.Common.Models;
using Tawreed.Application.Interfaces;

namespace Tawreed.Web.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    // Dashboard

    [HttpGet("dashboard")]
    public async Task<ActionResult<AdminDashboardData>> GetDashboard(CancellationToken cancellationToken)
    {
        var data = await _adminService.GetDashboardAsync(cancellationToken);
        return Ok(data);
    }

    // Users

    [HttpGet("users")]
    public async Task<ActionResult<PaginatedResult<AdminUserListDto>>> GetUsers(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _adminService.GetUsersAsync(search, status, page, limit, cancellationToken);
        return Ok(result);
    }

    [HttpGet("users/{userId:guid}")]
    public async Task<ActionResult<AdminUserDetailDto>> GetUserDetail(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _adminService.GetUserDetailAsync(userId, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "User not found" });
        }
    }

    [HttpPost("users/{userId:guid}/suspend")]
    public async Task<ActionResult> SuspendUser(Guid userId, [FromBody] string reason, CancellationToken cancellationToken)
    {
        try
        {
            await _adminService.SuspendUserAsync(userId, reason, cancellationToken);
            return Ok(new { message = "User suspended" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "User not found" });
        }
    }

    [HttpPost("users/{userId:guid}/reactivate")]
    public async Task<ActionResult> ReactivateUser(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            await _adminService.ReactivateUserAsync(userId, cancellationToken);
            return Ok(new { message = "User reactivated" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "User not found" });
        }
    }

    [HttpPost("users/{userId:guid}/reset-password")]
    public async Task<ActionResult> ResetPassword(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            var tempPassword = await _adminService.ResetUserPasswordAsync(userId, cancellationToken);
            return Ok(new { tempPassword });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "User not found" });
        }
    }

    // Suppliers

    [HttpGet("suppliers")]
    public async Task<ActionResult<PaginatedResult<AdminSupplierListDto>>> GetSuppliers(
        [FromQuery] string? search,
        [FromQuery] string? status,
        [FromQuery] Guid? categoryId,
        [FromQuery] Guid? regionId,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _adminService.GetSuppliersAsync(search, status, categoryId, regionId, page, limit, cancellationToken);
        return Ok(result);
    }

    [HttpGet("suppliers/{supplierId:guid}")]
    public async Task<ActionResult<AdminSupplierDetailDto>> GetSupplierDetail(Guid supplierId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _adminService.GetSupplierDetailAsync(supplierId, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Supplier not found" });
        }
    }

    [HttpPost("suppliers/{supplierId:guid}/approve")]
    public async Task<ActionResult> ApproveSupplier(Guid supplierId, CancellationToken cancellationToken)
    {
        try
        {
            var adminUserId = GetUserId();
            await _adminService.ApproveSupplierAsync(supplierId, adminUserId, cancellationToken);
            return Ok(new { message = "Supplier approved" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPost("suppliers/{supplierId:guid}/reject")]
    public async Task<ActionResult> RejectSupplier(Guid supplierId, [FromBody] string reason, CancellationToken cancellationToken)
    {
        try
        {
            var adminUserId = GetUserId();
            await _adminService.RejectSupplierAsync(supplierId, adminUserId, reason, cancellationToken);
            return Ok(new { message = "Supplier rejected" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPost("suppliers/{supplierId:guid}/suspend")]
    public async Task<ActionResult> SuspendSupplier(Guid supplierId, [FromBody] string reason, CancellationToken cancellationToken)
    {
        try
        {
            var adminUserId = GetUserId();
            await _adminService.SuspendSupplierAsync(supplierId, adminUserId, reason, cancellationToken);
            return Ok(new { message = "Supplier suspended" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPost("suppliers/{supplierId:guid}/reactivate")]
    public async Task<ActionResult> ReactivateSupplier(Guid supplierId, CancellationToken cancellationToken)
    {
        try
        {
            var adminUserId = GetUserId();
            await _adminService.ReactivateSupplierAsync(supplierId, adminUserId, cancellationToken);
            return Ok(new { message = "Supplier reactivated" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    // Orders

    [HttpGet("orders")]
    public async Task<ActionResult<PaginatedResult<AdminOrderListDto>>> GetOrders(
        [FromQuery] string? status,
        [FromQuery] Guid? regionId,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _adminService.GetOrdersAsync(status, regionId, page, limit, cancellationToken);
        return Ok(result);
    }

    [HttpGet("orders/{orderId:guid}")]
    public async Task<ActionResult> GetOrderDetail(Guid orderId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _adminService.GetOrderDetailAsync(orderId, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Order not found" });
        }
    }

    [HttpPost("orders/{orderId:guid}/force-close")]
    public async Task<ActionResult> ForceCloseOrder(Guid orderId, [FromBody] string reason, CancellationToken cancellationToken)
    {
        try
        {
            var adminUserId = GetUserId();
            await _adminService.ForceCloseOrderAsync(orderId, adminUserId, reason, cancellationToken);
            return Ok(new { message = "Order closed" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    // Categories

    [HttpGet("categories")]
    public async Task<ActionResult<PaginatedResult<AdminCategoryListDto>>> GetCategories(
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _adminService.GetCategoriesAsync(search, page, limit, cancellationToken);
        return Ok(result);
    }

    [HttpGet("categories/{categoryId:guid}")]
    public async Task<ActionResult> GetCategoryDetail(Guid categoryId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _adminService.GetCategoryDetailAsync(categoryId, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Category not found" });
        }
    }

    [HttpPost("categories")]
    public async Task<ActionResult<CategoryDto>> CreateCategory(
        [FromQuery] string nameAr,
        [FromQuery] string nameEn,
        [FromQuery] Guid? parentId,
        [FromQuery] string? iconUrl,
        [FromQuery] int sortOrder = 0,
        CancellationToken cancellationToken = default)
    {
        var category = await _adminService.CreateCategoryAsync(nameAr, nameEn, parentId, iconUrl, sortOrder, cancellationToken);
        return CreatedAtAction(nameof(GetCategoryDetail), new { categoryId = category.Id }, category);
    }

    [HttpPatch("categories/{categoryId:guid}")]
    public async Task<ActionResult> UpdateCategory(Guid categoryId,
        [FromBody] UpdateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            await _adminService.UpdateCategoryAsync(categoryId, request.NameAr, request.NameEn, request.ParentId, request.IconUrl, request.SortOrder, request.IsActive, cancellationToken);
            return Ok(new { message = "Category updated" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Category not found" });
        }
    }

    [HttpDelete("categories/{categoryId:guid}")]
    public async Task<ActionResult> DeactivateCategory(Guid categoryId, CancellationToken cancellationToken)
    {
        try
        {
            await _adminService.DeactivateCategoryAsync(categoryId, cancellationToken);
            return Ok(new { message = "Category deactivated" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Category not found" });
        }
    }

    [HttpPost("categories/{categoryId:guid}/activate")]
    public async Task<ActionResult> ActivateCategory(Guid categoryId, CancellationToken cancellationToken)
    {
        try
        {
            await _adminService.ActivateCategoryAsync(categoryId, cancellationToken);
            return Ok(new { message = "Category activated" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Category not found" });
        }
    }

    [HttpDelete("categories/{categoryId:guid}/delete")]
    public async Task<ActionResult> DeleteCategory(Guid categoryId, CancellationToken cancellationToken)
    {
        try
        {
            await _adminService.SoftDeleteCategoryAsync(categoryId, cancellationToken);
            return Ok(new { message = "Category deleted" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Category not found" });
        }
    }

    // Regions

    [HttpGet("regions")]
    public async Task<ActionResult<List<RegionDto>>> GetRegions(
        [FromQuery] string? search,
        CancellationToken cancellationToken)
    {
        var regions = await _adminService.GetRegionsAsync(search, cancellationToken);
        return Ok(regions);
    }

    [HttpPost("regions")]
    public async Task<ActionResult<RegionDto>> CreateRegion(
        [FromQuery] string nameAr,
        [FromQuery] string nameEn,
        [FromQuery] Guid? parentId,
        [FromQuery] string type,
        CancellationToken cancellationToken)
    {
        var region = await _adminService.CreateRegionAsync(nameAr, nameEn, parentId, type, cancellationToken);
        return CreatedAtAction(nameof(GetRegions), null, region);
    }

    [HttpPost("regions/{regionId:guid}/toggle")]
    public async Task<ActionResult> ToggleRegion(Guid regionId, CancellationToken cancellationToken)
    {
        try
        {
            await _adminService.ToggleRegionAsync(regionId, cancellationToken);
            return Ok(new { message = "Region toggled" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Region not found" });
        }
    }

    [HttpGet("regions/tree")]
    public async Task<ActionResult<List<RegionTreeNodeDto>>> GetRegionTree(CancellationToken cancellationToken)
    {
        var tree = await _adminService.GetRegionTreeAsync(cancellationToken);
        return Ok(tree);
    }

    [HttpGet("regions/stats")]
    public async Task<ActionResult<RegionStatsDto>> GetRegionStats(CancellationToken cancellationToken)
    {
        var stats = await _adminService.GetRegionStatsAsync(cancellationToken);
        return Ok(stats);
    }

    [HttpGet("regions/roots")]
    public async Task<ActionResult<List<RegionTreeNodeDto>>> GetRegionRoots(CancellationToken cancellationToken)
    {
        var roots = await _adminService.GetRegionRootsAsync(cancellationToken);
        return Ok(roots);
    }

    [HttpGet("regions/{regionId:guid}/children")]
    public async Task<ActionResult<List<RegionTreeNodeDto>>> GetRegionChildren(Guid regionId, CancellationToken cancellationToken)
    {
        var children = await _adminService.GetRegionChildrenAsync(regionId, cancellationToken);
        return Ok(children);
    }

    [HttpPut("regions/{regionId:guid}")]
    public async Task<ActionResult> UpdateRegion(Guid regionId,
        [FromBody] UpdateRegionRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var region = await _adminService.UpdateRegionAsync(regionId, request.NameAr, request.NameEn, request.ParentId, request.Type, cancellationToken);
            return Ok(region);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Region not found" });
        }
    }

    [HttpDelete("regions/{regionId:guid}")]
    public async Task<ActionResult> DeleteRegion(Guid regionId, CancellationToken cancellationToken)
    {
        try
        {
            await _adminService.DeleteRegionAsync(regionId, cancellationToken);
            return Ok(new { message = "Region deleted" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Region not found" });
        }
    }

    // Settings

    [HttpGet("settings/group-region-types")]
    public async Task<ActionResult<List<string>>> GetGroupRegionTypes(CancellationToken cancellationToken)
    {
        var types = await _adminService.GetGroupRegionTypesAsync(cancellationToken);
        return Ok(types);
    }

    [HttpPut("settings/group-region-types")]
    public async Task<ActionResult> SetGroupRegionTypes([FromBody] List<string> types, CancellationToken cancellationToken)
    {
        await _adminService.SetGroupRegionTypesAsync(types, cancellationToken);
        return Ok(new { message = "Group region types updated" });
    }

    // Profile

    [HttpGet("profile")]
    public async Task<ActionResult<AdminProfileDto>> GetProfile(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        try
        {
            var profile = await _adminService.GetProfileAsync(userId, cancellationToken);
            return Ok(profile);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Admin profile not found" });
        }
    }

    [HttpPatch("profile")]
    public async Task<ActionResult> UpdateProfile(
        [FromBody] UpdateAdminProfileRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        try
        {
            await _adminService.UpdateProfileAsync(userId, request, cancellationToken);
            return Ok(new { message = "Profile updated" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Admin profile not found" });
        }
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        return Guid.Parse(claim!.Value);
    }
}

public record UpdateCategoryRequest(
    string? NameAr = null,
    string? NameEn = null,
    Guid? ParentId = null,
    string? IconUrl = null,
    int? SortOrder = null,
    bool? IsActive = null
);

public record UpdateRegionRequest(
    string NameAr,
    string NameEn,
    Guid? ParentId,
    string? Type = null
);
