namespace Tawreed.Application.Interfaces;

public class BuyerProfileDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string BusinessName { get; set; } = string.Empty;
    public string BusinessType { get; set; } = string.Empty;
    public string? TaxId { get; set; }
    public string? Avatar { get; set; }
    public DateTimeOffset JoinedDate { get; set; }
    public string? Address { get; set; }
    public string RegionName { get; set; } = string.Empty;
    public Guid RegionId { get; set; }
    public string PreferredLang { get; set; } = string.Empty;
}

public record BuyerUpdateProfileRequest(
    string? FullName,
    string? Phone,
    string? BusinessName,
    string? BusinessType,
    string? TaxId,
    string? Address,
    Guid? RegionId,
    string? Avatar,
    string? PreferredLang
);

public interface IBuyerProfileService
{
    Task<BuyerProfileDto> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default);
    Task UpdateProfileAsync(Guid userId, BuyerUpdateProfileRequest request, CancellationToken cancellationToken = default);
}
