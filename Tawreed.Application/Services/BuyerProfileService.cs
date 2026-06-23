using Tawreed.Application.Interfaces;
using Tawreed.Domain.Interfaces;

namespace Tawreed.Application.Services;

public class BuyerProfileService : IBuyerProfileService
{
    private readonly IBuyerRepository _buyerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BuyerProfileService(IBuyerRepository buyerRepository, IUnitOfWork unitOfWork)
    {
        _buyerRepository = buyerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BuyerProfileDto> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var buyer = await _buyerRepository.GetByUserIdWithDetailsAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("Buyer profile not found.");

        return new BuyerProfileDto
        {
            Name = buyer.User?.FullName ?? "",
            Email = buyer.User?.Email ?? "",
            Phone = buyer.User?.Phone ?? "",
            BusinessName = buyer.BusinessName,
            BusinessType = buyer.BusinessType,
            TaxId = buyer.TaxId,
            Avatar = buyer.User?.AvatarUrl,
            JoinedDate = buyer.CreatedAt,
            Address = buyer.Address,
            RegionName = buyer.Region?.NameEn ?? "",
            RegionId = buyer.RegionId,
            PreferredLang = buyer.User?.PreferredLang ?? "en"
        };
    }

    public async Task UpdateProfileAsync(Guid userId, BuyerUpdateProfileRequest request, CancellationToken cancellationToken = default)
    {
        var buyer = await _buyerRepository.GetByUserIdWithDetailsAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("Buyer profile not found.");

        var user = buyer.User;
        if (user == null) throw new KeyNotFoundException("User not found.");

        if (request.FullName != null) user.FullName = request.FullName;
        if (request.Phone != null) user.Phone = request.Phone;
        if (request.Avatar != null) user.AvatarUrl = request.Avatar;
        if (request.PreferredLang != null) user.PreferredLang = request.PreferredLang;
        if (request.BusinessName != null) buyer.BusinessName = request.BusinessName;
        if (request.BusinessType != null) buyer.BusinessType = request.BusinessType;
        if (request.TaxId != null) buyer.TaxId = request.TaxId;
        if (request.Address != null) buyer.Address = request.Address;
        if (request.RegionId.HasValue) buyer.RegionId = request.RegionId.Value;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
