using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tawreed.Application.Common.Models;
using Tawreed.Application.Interfaces;
using Tawreed.Domain.Interfaces;

namespace Tawreed.Web.Controllers;

[Authorize]
[ApiController]
[Route("api/supplier")]
public class SupplierController : ControllerBase
{
    private readonly ISupplierDashboardService _dashboardService;
    private readonly ISupplierOrderService _orderService;
    private readonly ISupplierProfileService _profileService;
    private readonly ISupplierProductService _productService;
    private readonly IPricingTierService _pricingTierService;
    private readonly ISupplierRepository _supplierRepository;

    public SupplierController(
        ISupplierDashboardService dashboardService,
        ISupplierOrderService orderService,
        ISupplierProfileService profileService,
        ISupplierProductService productService,
        IPricingTierService pricingTierService,
        ISupplierRepository supplierRepository)
    {
        _dashboardService = dashboardService;
        _orderService = orderService;
        _profileService = profileService;
        _productService = productService;
        _pricingTierService = pricingTierService;
        _supplierRepository = supplierRepository;
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult<SupplierDashboardData>> GetDashboard(CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var data = await _dashboardService.GetDashboardAsync(userId, cancellationToken);
            return Ok(data);
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(403, new { error = ex.Message });
        }
    }

    [HttpGet("profile")]
    public async Task<ActionResult<SupplierProfileDto>> GetProfile(CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var profile = await _profileService.GetProfileAsync(userId, cancellationToken);
            return Ok(profile);
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(403, new { error = ex.Message });
        }
    }

    [HttpPut("profile")]
    public async Task<ActionResult> UpdateProfile(UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            await _profileService.UpdateProfileAsync(userId, request, cancellationToken);
            return Ok(new { message = "Profile updated" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(403, new { error = ex.Message });
        }
    }

    [HttpGet("profile/categories")]
    public async Task<ActionResult<List<Guid>>> GetCategoryIds(CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var ids = await _profileService.GetCategoryIdsAsync(userId, cancellationToken);
            return Ok(ids);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(403, new { error = ex.Message });
        }
    }

    [HttpPut("profile/categories")]
    public async Task<ActionResult> UpdateCategories([FromBody] List<Guid> categoryIds, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            await _profileService.UpdateCategoriesAsync(userId, categoryIds, cancellationToken);
            return Ok(new { message = "Categories updated" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(403, new { error = ex.Message });
        }
    }

    [HttpGet("registration-status")]
    public async Task<ActionResult<SupplierRegistrationStatusDto>> GetRegistrationStatus(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var status = await _profileService.GetRegistrationStatusAsync(userId, cancellationToken);
        return Ok(status);
    }

    [HttpGet("orders")]
    public async Task<ActionResult<PaginatedResult<SupplierOrderListDto>>> GetOrders(
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = GetUserId();
            var result = await _orderService.GetOrdersAsync(userId, status, page, limit, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(403, new { error = ex.Message });
        }
    }

    [HttpPost("orders/{orderId:guid}/accept")]
    public async Task<ActionResult> AcceptOrder(Guid orderId, [FromBody] AcceptOrderRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var result = await _orderService.AcceptOrderAsync(orderId, userId, request, cancellationToken);
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

    [HttpPost("orders/{orderId:guid}/decline")]
    public async Task<ActionResult> DeclineOrder(Guid orderId, [FromBody] string reason, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var result = await _orderService.DeclineOrderAsync(orderId, userId, reason, cancellationToken);
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

    [HttpGet("deliveries")]
    public async Task<ActionResult<PaginatedResult<DeliveryListDto>>> GetDeliveries(
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = GetUserId();
            var result = await _orderService.GetDeliveriesAsync(userId, status, page, limit, cancellationToken);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(403, new { error = ex.Message });
        }
    }

    [HttpPatch("deliveries/{deliveryId:guid}/status")]
    public async Task<ActionResult> UpdateDeliveryStatus(Guid deliveryId,
        [FromBody] UpdateDeliveryStatusRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var result = await _orderService.UpdateDeliveryStatusAsync(deliveryId, request.Status, request.TrackingNotes, request.ScheduledAt, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpGet("orders/{orderId:guid}/available-delivery-persons")]
    public async Task<ActionResult> BrowseAvailableDeliveryPersons(Guid orderId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var result = await _orderService.BrowseAvailableDeliveryPersonsAsync(orderId, userId, cancellationToken);
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

    [HttpPost("orders/{orderId:guid}/request-delivery/{deliveryPersonId:guid}")]
    public async Task<ActionResult> RequestDeliveryPerson(Guid orderId, Guid deliveryPersonId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var result = await _orderService.RequestDeliveryPersonAsync(orderId, deliveryPersonId, userId, cancellationToken);
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

    [HttpPost("orders/{orderId:guid}/assign-delivery-person/{deliveryPersonId:guid}")]
    public async Task<ActionResult> AssignDeliveryPerson(Guid orderId, Guid deliveryPersonId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var result = await _orderService.AssignDeliveryPersonAsync(orderId, deliveryPersonId, userId, cancellationToken);
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

    [HttpPost("orders/{orderId:guid}/propose-delivery-fee")]
    public async Task<ActionResult> ProposeDeliveryFee(Guid orderId, [FromBody] ProposeDeliveryFeeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var result = await _orderService.ProposeDeliveryFeeAsync(orderId, request.Fee, request.Notes, userId, cancellationToken);
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

    [HttpPost("orders/{orderId:guid}/use-own-delivery")]
    public async Task<ActionResult> UseOwnDelivery(Guid orderId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var result = await _orderService.UseOwnDeliveryAsync(orderId, userId, cancellationToken);
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

    [HttpPost("orders/{orderId:guid}/cancel")]
    public async Task<ActionResult> CancelOrder(Guid orderId, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            var result = await _orderService.CancelOrderFromSupplierAsync(orderId, userId, cancellationToken);
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

    private async Task<Guid> GetSupplierIdAsync(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var supplier = await _supplierRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new InvalidOperationException("Supplier profile not found.");
        if (!supplier.IsApproved)
            throw new InvalidOperationException("Account pending approval.");
        return supplier.Id;
    }

    [HttpGet("products")]
    public async Task<ActionResult<IReadOnlyList<SupplierProductDto>>> GetProducts(CancellationToken cancellationToken)
    {
        try
        {
            var supplierId = await GetSupplierIdAsync(cancellationToken);
            var products = await _productService.GetBySupplierAsync(supplierId, cancellationToken);
            return Ok(products);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("products/{productId:guid}")]
    public async Task<ActionResult<SupplierProductDto>> GetProduct(Guid productId, CancellationToken cancellationToken)
    {
        var product = await _productService.GetByIdAsync(productId, cancellationToken);
        if (product is null)
            return NotFound(new { error = "Product not found." });
        return Ok(product);
    }

    [HttpPost("products")]
    public async Task<ActionResult<SupplierProductDto>> AddProduct(AddSupplierProductRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var supplierId = await GetSupplierIdAsync(cancellationToken);
            var product = await _productService.AddProductAsync(supplierId, request.ProductId, request.Price, request.Stock, request.Tiers, cancellationToken);
            return CreatedAtAction(nameof(GetProduct), new { productId = product.Id }, product);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("products/{productId:guid}")]
    public async Task<ActionResult<SupplierProductDto>> UpdateProduct(Guid productId, UpdateSupplierProductRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _productService.UpdateProductAsync(productId, request.Price, request.Stock, request.IsActive, cancellationToken);
            return Ok(product);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpDelete("products/{productId:guid}")]
    public async Task<ActionResult> DeleteProduct(Guid productId, CancellationToken cancellationToken)
    {
        await _productService.DeleteProductAsync(productId, cancellationToken);
        return NoContent();
    }

    [HttpGet("products/{productId:guid}/tiers")]
    public async Task<ActionResult<IReadOnlyList<PricingTierDto>>> GetTiers(Guid productId, CancellationToken cancellationToken)
    {
        try
        {
            var tiers = await _pricingTierService.GetByProductAsync(productId, cancellationToken);
            return Ok(tiers);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Product not found." });
        }
    }

    [HttpPost("products/{productId:guid}/tiers")]
    public async Task<ActionResult<PricingTierDto>> CreateTier(Guid productId, CreatePricingTierRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var tier = await _pricingTierService.CreateAsync(productId, request, cancellationToken);
            return CreatedAtAction(nameof(GetTiers), new { productId }, tier);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Product not found." });
        }
    }

    [HttpPut("products/{productId:guid}/tiers/{tierId:guid}")]
    public async Task<ActionResult<PricingTierDto>> UpdateTier(Guid productId, Guid tierId, UpdatePricingTierRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var tier = await _pricingTierService.UpdateAsync(tierId, request, cancellationToken);
            return Ok(tier);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Pricing tier not found." });
        }
    }

    [HttpDelete("products/{productId:guid}/tiers/{tierId:guid}")]
    public async Task<ActionResult> DeleteTier(Guid productId, Guid tierId, CancellationToken cancellationToken)
    {
        try
        {
            await _pricingTierService.DeleteAsync(tierId, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { error = "Pricing tier not found." });
        }
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        return Guid.Parse(claim!.Value);
    }
}

public record ProposeDeliveryFeeRequest(decimal Fee, string? Notes = null);
public record UpdateDeliveryStatusRequest(string Status, string? TrackingNotes = null, DateTimeOffset? ScheduledAt = null);
public record AddSupplierProductRequest(Guid ProductId, decimal Price, int Stock, List<CreatePricingTierRequest>? Tiers = null);
public record UpdateSupplierProductRequest(decimal? Price = null, int? Stock = null, bool? IsActive = null);
