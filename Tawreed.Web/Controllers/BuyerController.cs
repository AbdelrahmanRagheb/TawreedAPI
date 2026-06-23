using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tawreed.Application.Interfaces;

namespace Tawreed.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/buyer")]
public class BuyerController : ControllerBase
{
    private readonly IBuyerDashboardService _dashboardService;
    private readonly IBuyerOrderService _orderService;
    private readonly IBuyerProfileService _profileService;
    private readonly IBuyerSupplierService _supplierService;

    public BuyerController(
        IBuyerDashboardService dashboardService,
        IBuyerOrderService orderService,
        IBuyerProfileService profileService,
        IBuyerSupplierService supplierService)
    {
        _dashboardService = dashboardService;
        _orderService = orderService;
        _profileService = profileService;
        _supplierService = supplierService;
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult<BuyerDashboardData>> GetDashboard(
        [FromQuery] Guid? regionId, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var data = await _dashboardService.GetDashboardAsync(userId, regionId, cancellationToken);
        return Ok(data);
    }

    [HttpGet("orders")]
    public async Task<ActionResult<PaginatedResult<OrderListDto>>> GetOrders(
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20,
        CancellationToken cancellationToken = default)
    {
        var userId = GetUserId();
        var result = await _orderService.GetOrdersAsync(userId, status, page, limit, cancellationToken);
        return Ok(result);
    }

    [HttpGet("orders/{orderId:guid}")]
    public async Task<ActionResult<OrderDetailDto>> GetOrderDetail(Guid orderId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _orderService.GetOrderDetailAsync(orderId, cancellationToken);
            var userId = GetUserId();
            result.IsParticipant = result.Participants.Any(p => p.UserId == userId);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Order not found" });
        }
    }

    [HttpPost("orders")]
    public async Task<ActionResult> CreateOrder(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var order = await _orderService.CreateOrderAsync(userId, request, cancellationToken);
        return CreatedAtAction(nameof(GetOrderDetail), new { orderId = order.Id }, order);
    }

    [HttpPost("orders/draft")]
    public async Task<ActionResult> SaveDraft(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var order = await _orderService.SaveDraftAsync(userId, request, cancellationToken);
        return CreatedAtAction(nameof(GetOrderDetail), new { orderId = order.Id }, order);
    }

    [HttpGet("orders/drafts")]
    public async Task<ActionResult<PaginatedResult<OrderListDto>>> GetDrafts(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20,
        CancellationToken cancellationToken = default)
    {
        var userId = GetUserId();
        var result = await _orderService.GetDraftsAsync(userId, page, limit, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("orders/{orderId:guid}/draft")]
    public async Task<ActionResult> DeleteDraft(Guid orderId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            await _orderService.DeleteDraftAsync(orderId, userId, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Draft not found" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("orders/{orderId:guid}/join")]
    public async Task<ActionResult> JoinOrder(Guid orderId, JoinOrderRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var result = await _orderService.JoinOrderAsync(orderId, userId, request, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("orders/{orderId:guid}/leave")]
    public async Task<ActionResult> LeaveOrder(Guid orderId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var result = await _orderService.LeaveOrderAsync(orderId, userId, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("orders/{orderId:guid}/participants/{participantId:guid}/items")]
    public async Task<ActionResult> UpdateItems(Guid orderId, Guid participantId, UpdateItemsRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var result = await _orderService.UpdateItemsAsync(orderId, participantId, userId, request, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }

    [HttpGet("profile")]
    public async Task<ActionResult<BuyerProfileDto>> GetProfile(CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var profile = await _profileService.GetProfileAsync(userId, cancellationToken);
            return Ok(profile);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Buyer profile not found" });
        }
    }

    [HttpPut("profile")]
    public async Task<ActionResult> UpdateProfile(BuyerUpdateProfileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            await _profileService.UpdateProfileAsync(userId, request, cancellationToken);
            return Ok(new { message = "Profile updated successfully" });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Buyer profile not found" });
        }
    }

    [HttpGet("orders/{orderId:guid}/eligible-suppliers")]
    public async Task<ActionResult<IReadOnlyList<EligibleSupplierDto>>> GetEligibleSuppliers(Guid orderId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var suppliers = await _orderService.GetEligibleSuppliersAsync(orderId, userId, cancellationToken);
            return Ok(suppliers);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpGet("orders/{orderId:guid}/suppliers/{supplierId:guid}")]
    public async Task<ActionResult<SupplierPublicProfileDto>> GetSupplierProfile(Guid orderId, Guid supplierId, CancellationToken cancellationToken)
    {
        try
        {
            var profile = await _supplierService.GetSupplierProfileAsync(supplierId, orderId, cancellationToken);
            return Ok(profile);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPut("orders/{orderId:guid}/items")]
    public async Task<ActionResult> UpdateOrderItems(Guid orderId, [FromBody] List<CreateOrderItem> items, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var result = await _orderService.UpdateOrderItemsAsync(orderId, userId, items, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        return Guid.Parse(claim!.Value);
    }
}
