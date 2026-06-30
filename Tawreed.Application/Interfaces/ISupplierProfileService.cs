using Tawreed.Application.Common.Models;

namespace Tawreed.Application.Interfaces;

public class SupplierProfileDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string? TaxId { get; set; }
    public string? CommercialRegistrationNo { get; set; }
    public string? Avatar { get; set; }
    public DateTimeOffset JoinedDate { get; set; }
    public string RegionName { get; set; } = string.Empty;
    public Guid RegionId { get; set; }
    public bool IsApproved { get; set; }
    public string PreferredLang { get; set; } = string.Empty;
    public required List<Guid> CategoryIds { get; set; }
}

public class SupplierRegistrationStatusDto
{
    public string Status { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public required List<ApprovalLogEntry> ApprovalLogs { get; set; }
}

public class ApprovalLogEntry
{
    public string Action { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public string ActorName { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
}

public interface ISupplierProfileService
{
    Task<SupplierProfileDto> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default);
    Task UpdateProfileAsync(Guid userId, UpdateProfileRequest request, CancellationToken cancellationToken = default);
    Task<SupplierRegistrationStatusDto> GetRegistrationStatusAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<Guid>> GetCategoryIdsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task UpdateCategoriesAsync(Guid userId, List<Guid> categoryIds, CancellationToken cancellationToken = default);
}
