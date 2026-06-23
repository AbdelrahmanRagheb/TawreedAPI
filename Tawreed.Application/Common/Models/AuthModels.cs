namespace Tawreed.Application.Common.Models;

public record LoginRequest(string Email, string Password);

public record RegisterBuyerRequest(
    string FullName,
    string Email,
    string Phone,
    string Password,
    string BusinessName,
    string BusinessType,
    Guid RegionId,
    string? Address
);

public record RegisterSupplierRequest(
    string FullName,
    string Email,
    string Phone,
    string Password,
    string CompanyName,
    string? TaxId,
    Guid RegionId,
    List<Guid> CategoryIds
);

public record RefreshTokenRequest(string RefreshToken);

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);

public record AuthResponse(
    string Token,
    string RefreshToken,
    UserInfo User
);

public record UserInfo(
    Guid Id,
    string Name,
    string Email,
    string Role,
    string? Avatar,
    string PreferredLang = "en"
);

public record UpdateProfileRequest(
    string? FullName,
    string? Phone,
    string? BusinessName,
    string? Address,
    string? Avatar,
    string? PreferredLang
);
