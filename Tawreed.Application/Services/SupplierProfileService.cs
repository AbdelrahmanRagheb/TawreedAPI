using Tawreed.Application.Common.Models;
using Tawreed.Application.Interfaces;
using Tawreed.Domain.Entities;
using Tawreed.Domain.Interfaces;

namespace Tawreed.Application.Services;

public class SupplierProfileService : ISupplierProfileService
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly ISupplierCategoryRepository _supplierCategoryRepository;
    private readonly ISupplierApprovalLogRepository _approvalLogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SupplierProfileService(
        ISupplierRepository supplierRepository,
        ISupplierCategoryRepository supplierCategoryRepository,
        ISupplierApprovalLogRepository approvalLogRepository,
        IUnitOfWork unitOfWork)
    {
        _supplierRepository = supplierRepository;
        _supplierCategoryRepository = supplierCategoryRepository;
        _approvalLogRepository = approvalLogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<SupplierProfileDto> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var supplier = await _supplierRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("Supplier profile not found.");
        if (!supplier.IsApproved)
            throw new InvalidOperationException("Account pending approval.");

        var detailed = await _supplierRepository.GetByIdWithDetailsAsync(supplier.Id, cancellationToken);

        return new SupplierProfileDto
        {
            Name = supplier.User?.FullName ?? "",
            Email = supplier.User?.Email ?? "",
            Phone = supplier.User?.Phone ?? "",
            CompanyName = supplier.CompanyName,
            TaxId = supplier.TaxId,
            Avatar = supplier.User?.AvatarUrl,
            JoinedDate = supplier.CreatedAt,
            Address = supplier.Address,
            RegionName = supplier.Region?.NameEn ?? "",
            RegionId = supplier.RegionId,
            RatingAvg = supplier.RatingAvg,
            IsApproved = supplier.IsApproved,
            PreferredLang = supplier.User?.PreferredLang ?? "en",
            CategoryIds = detailed?.SupplierCategories?.Select(sc => sc.CategoryId).ToList() ?? []
        };
    }

    public async Task UpdateProfileAsync(Guid userId, UpdateProfileRequest request, CancellationToken cancellationToken = default)
    {
        var supplier = await _supplierRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("Supplier profile not found.");
        if (!supplier.IsApproved)
            throw new InvalidOperationException("Account pending approval.");

        var user = supplier.User;
        if (user == null) throw new KeyNotFoundException("User not found.");

        if (request.FullName != null) user.FullName = request.FullName;
        if (request.Phone != null) user.Phone = request.Phone;
        if (request.Avatar != null) user.AvatarUrl = request.Avatar;
        if (request.PreferredLang != null) user.PreferredLang = request.PreferredLang;
        if (request.BusinessName != null) supplier.CompanyName = request.BusinessName;
        if (request.Address != null) supplier.Address = request.Address;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Guid>> GetCategoryIdsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var supplier = await _supplierRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("Supplier profile not found.");

        var categories = await _supplierCategoryRepository.GetBySupplierAsync(supplier.Id, cancellationToken);
        return categories.Select(sc => sc.CategoryId).ToList();
    }

    public async Task UpdateCategoriesAsync(Guid userId, List<Guid> categoryIds, CancellationToken cancellationToken = default)
    {
        var supplier = await _supplierRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("Supplier profile not found.");
        if (!supplier.IsApproved)
            throw new InvalidOperationException("Account pending approval.");

        var existing = await _supplierCategoryRepository.GetBySupplierAsync(supplier.Id, cancellationToken);
        var existingIds = existing.Select(sc => sc.CategoryId).ToHashSet();

        foreach (var sc in existing.Where(sc => !categoryIds.Contains(sc.CategoryId)).ToList())
            _supplierCategoryRepository.Delete(sc);

        foreach (var id in categoryIds.Where(id => !existingIds.Contains(id)))
            _supplierCategoryRepository.Add(new SupplierCategory { SupplierId = supplier.Id, CategoryId = id });

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<SupplierRegistrationStatusDto> GetRegistrationStatusAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var supplier = await _supplierRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("Supplier profile not found.");

        var logs = await _approvalLogRepository.GetBySupplierAsync(supplier.Id, cancellationToken);

        return new SupplierRegistrationStatusDto
        {
            Status = supplier.User?.Status ?? "PendingApproval",
            IsApproved = supplier.IsApproved,
            ApprovalLogs = logs.Select(l => new ApprovalLogEntry
            {
                Action = l.Action,
                Reason = l.Reason,
                ActorName = l.Actor?.FullName ?? "",
                CreatedAt = l.CreatedAt
            }).OrderByDescending(l => l.CreatedAt).ToList()
        };
    }
}
