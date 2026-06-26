namespace Tawreed.Application.Common.Models;

public class RegisterDeliveryPersonRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string VehicleType { get; set; } = string.Empty;
    public string? LicenseInfo { get; set; }
    public decimal BaseDeliveryFee { get; set; }
    public Guid? CoverageRegionId { get; set; }
}
