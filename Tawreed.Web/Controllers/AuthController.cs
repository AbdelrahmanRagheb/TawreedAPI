using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tawreed.Application.Common.Models;
using Tawreed.Application.Interfaces;

namespace Tawreed.Web.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _authService.LoginAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }

    [HttpPost("register/buyer")]
    public async Task<ActionResult<AuthResponse>> RegisterBuyer(RegisterBuyerRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _authService.RegisterBuyerAsync(request, cancellationToken);
            return CreatedAtAction(nameof(Login), response);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    [HttpPost("register/supplier")]
    public async Task<ActionResult<AuthResponse>> RegisterSupplier(RegisterSupplierRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _authService.RegisterSupplierAsync(request, cancellationToken);
            return CreatedAtAction(nameof(Login), response);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    [HttpPost("register/delivery-person")]
    public async Task<ActionResult<AuthResponse>> RegisterDeliveryPerson(RegisterDeliveryPersonRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _authService.RegisterDeliveryPersonAsync(request, cancellationToken);
            return CreatedAtAction(nameof(Login), response);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult> Logout(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        await _authService.LogoutAsync(userId, cancellationToken);
        return Ok(new { message = "Logged out" });
    }

    #if DEBUG
    [HttpPost("debug-login")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<AuthResponse>> DebugLogin(LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _authService.DebugLoginAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }
    #endif

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> Refresh(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _authService.RefreshTokenAsync(request.RefreshToken, cancellationToken);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }

    [HttpPut("password")]
    [Authorize]
    public async Task<ActionResult> ChangePassword(ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var userId = GetUserId();
            await _authService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword, cancellationToken);
            return Ok(new { message = "Password updated" });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserInfo>> GetCurrentUser(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var user = await _authService.GetCurrentUserAsync(userId, cancellationToken);
        return Ok(user);
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        return Guid.Parse(claim!.Value);
    }
}
