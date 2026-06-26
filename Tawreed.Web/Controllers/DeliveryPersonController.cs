using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tawreed.Application.Common.Models;
using Tawreed.Application.Interfaces;

namespace Tawreed.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/delivery-person")]
public class DeliveryPersonController : ControllerBase
{
    private readonly IDeliveryPersonService _deliveryPersonService;

    public DeliveryPersonController(IDeliveryPersonService deliveryPersonService)
    {
        _deliveryPersonService = deliveryPersonService;
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult<DeliveryPersonDashboardDto>> GetDashboard(CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var data = await _deliveryPersonService.GetDashboardAsync(userId, cancellationToken);
            return Ok(data);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }

    [HttpGet("deliveries")]
    public async Task<ActionResult<PaginatedResult<DeliveryPersonDeliveryDto>>> GetMyDeliveries(
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = GetUserId();
            var result = await _deliveryPersonService.GetMyDeliveriesAsync(userId, status, page, limit, cancellationToken);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }

    [HttpGet("deliveries/{deliveryId:guid}")]
    public async Task<ActionResult<DeliveryPersonDeliveryDetailDto>> GetDeliveryDetail(Guid deliveryId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var result = await _deliveryPersonService.GetDeliveryDetailAsync(deliveryId, userId, cancellationToken);
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

    [HttpPatch("deliveries/{deliveryId:guid}/status")]
    public async Task<ActionResult> UpdateDeliveryStatus(Guid deliveryId, [FromBody] DeliveryPersonUpdateStatusRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var result = await _deliveryPersonService.UpdateDeliveryStatusAsync(userId, deliveryId, request.Status, request.TrackingNotes, cancellationToken);
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

    [HttpPost("deliveries/{deliveryId:guid}/verify/{invoiceId:guid}")]
    public async Task<ActionResult> VerifyDelivery(Guid deliveryId, Guid invoiceId, [FromBody] VerifyDeliveryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var result = await _deliveryPersonService.VerifyDeliveryAsync(userId, invoiceId, request.VerificationCode, cancellationToken);
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

    [HttpGet("profile")]
    public async Task<ActionResult<DeliveryPersonProfileDto>> GetProfile(CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var profile = await _deliveryPersonService.GetProfileAsync(userId, cancellationToken);
            return Ok(profile);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPut("profile")]
    public async Task<ActionResult> UpdateProfile(UpdateDeliveryPersonProfileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            await _deliveryPersonService.UpdateProfileAsync(userId, request, cancellationToken);
            return Ok(new { message = "Profile updated" });
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

    [HttpGet("available-regions")]
    public async Task<ActionResult<IReadOnlyList<AvailableRegionDto>>> GetAvailableRegions(CancellationToken cancellationToken)
    {
        try
        {
            var regions = await _deliveryPersonService.GetAvailableRegionsAsync(cancellationToken);
            return Ok(regions);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("pending-requests")]
    public async Task<ActionResult<IReadOnlyList<PendingDeliveryRequestDto>>> GetPendingRequests(CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var requests = await _deliveryPersonService.GetPendingRequestsAsync(userId, cancellationToken);
            return Ok(requests);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPost("accept-request/{requestId:guid}")]
    public async Task<ActionResult> AcceptRequest(Guid requestId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var result = await _deliveryPersonService.AcceptDeliveryRequestAsync(requestId, userId, cancellationToken);
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

    [HttpPost("decline-request/{requestId:guid}")]
    public async Task<ActionResult> DeclineRequest(Guid requestId, [FromBody] DeclineRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var result = await _deliveryPersonService.DeclineDeliveryRequestAsync(requestId, userId, request.Reason, cancellationToken);
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

    private Guid GetUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        return Guid.Parse(claim!.Value);
    }
}

public record DeliveryPersonUpdateStatusRequest(string Status, string? TrackingNotes = null);
public record VerifyDeliveryRequest(string VerificationCode);
public record DeclineRequestDto(string? Reason = null);