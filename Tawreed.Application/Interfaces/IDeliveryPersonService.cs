using Tawreed.Domain.Entities;

namespace Tawreed.Application.Interfaces
{
    public interface IDeliveryPersonService
    {
        Task<DeliveryPersonDashboardDto> GetDashboardAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<PaginatedResult<DeliveryPersonDeliveryDto>> GetMyDeliveriesAsync(Guid userId, string? status, int page, int limit, CancellationToken cancellationToken = default);
        Task<DeliveryPersonDeliveryDetailDto> GetDeliveryDetailAsync(Guid deliveryId, Guid userId, CancellationToken cancellationToken = default);
        Task<object> UpdateDeliveryStatusAsync(Guid userId, Guid deliveryId, string status, string? trackingNotes = null, CancellationToken cancellationToken = default);
        Task<object> VerifyDeliveryAsync(Guid userId, Guid invoiceId, string verificationCode, CancellationToken cancellationToken = default);
        Task<DeliveryPersonProfileDto> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default);
        Task UpdateProfileAsync(Guid userId, UpdateDeliveryPersonProfileRequest request, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<AvailableRegionDto>> GetAvailableRegionsAsync(CancellationToken cancellationToken = default);

        // Delivery assignment requests
        Task<IReadOnlyList<PendingDeliveryRequestDto>> GetPendingRequestsAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<object> AcceptDeliveryRequestAsync(Guid requestId, Guid userId, CancellationToken cancellationToken = default);
        Task<object> DeclineDeliveryRequestAsync(Guid requestId, Guid userId, string? reason, CancellationToken cancellationToken = default);
    }

    public class PendingDeliveryRequestDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string OrderTitle { get; set; } = string.Empty;
        public string CreatorName { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public decimal? ProposedFee { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? RespondedAt { get; set; }
    }

    public class AvailableRegionDto
    {
        public Guid Id { get; set; }
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
    }

    public class DeliveryPersonDashboardDto
    {
        public int ActiveDeliveries { get; set; }
        public int CompletedToday { get; set; }
        public decimal Rating { get; set; }
        public decimal EarningsToday { get; set; }
    }

    public class DeliveryPersonDeliveryDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public string OrderTitle { get; set; } = string.Empty;
        public string ShippingRegion { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime? ScheduledAt { get; set; }
        public List<DeliveryPersonDeliveryParticipantDto> Participants { get; set; } = new List<DeliveryPersonDeliveryParticipantDto>();
    }

    public class DeliveryPersonDeliveryParticipantDto
    {
        public Guid InvoiceId { get; set; }
        public Guid ParticipantId { get; set; }
        public string ParticipantName { get; set; } = string.Empty;
        public string Status { get; set; } = "Joined";
    }

    public class DeliveryPersonDeliveryDetailDto : DeliveryPersonDeliveryDto
    {
        public List<DeliveryPersonDeliveryParticipantDetailDto> ParticipantDetails { get; set; } = new List<DeliveryPersonDeliveryParticipantDetailDto>();
    }

    public class DeliveryPersonDeliveryParticipantDetailDto
    {
        public Guid InvoiceId { get; set; }
        public Guid ParticipantId { get; set; }
        public string ParticipantName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Status { get; set; } = "Joined";
        public string? VerificationCode { get; set; } = string.Empty;
        public List<DeliveryPersonDeliveryItemDto> Items { get; set; } = new List<DeliveryPersonDeliveryItemDto>();
    }

    public class DeliveryPersonDeliveryItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Size { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class DeliveryPersonProfileDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string VehicleType { get; set; } = string.Empty;
        public decimal BaseDeliveryFee { get; set; }
        public decimal Rating { get; set; }
        public int TotalDeliveries { get; set; }
        public bool IsActive { get; set; }
        public Guid? CoverageRegionId { get; set; }
        public string? CoverageRegionName { get; set; }
    }

    public class UpdateDeliveryPersonProfileRequest
    {
        public string? VehicleType { get; set; }
        public decimal? BaseDeliveryFee { get; set; }
        public Guid? CoverageRegionId { get; set; }
    }
}
