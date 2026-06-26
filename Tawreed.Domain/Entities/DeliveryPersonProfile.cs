using Tawreed.Domain.Common;

namespace Tawreed.Domain.Entities;

public class DeliveryPersonProfile : BaseAuditableEntity
{
    public Guid UserId { get; set; }
    public string VehicleType { get; set; } = string.Empty;
    public string? LicenseInfo { get; set; }
    public decimal BaseDeliveryFee { get; set; }
    public Guid? CoverageRegionId { get; set; }
    public decimal Rating { get; set; }
    public int TotalDeliveries { get; set; }
    public bool IsActive { get; set; } = true;

    public User User { get; set; } = null!;
    public Region? CoverageRegion { get; set; }
}
