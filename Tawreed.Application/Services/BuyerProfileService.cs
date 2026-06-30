using System.Text.Json;
using Tawreed.Application.Interfaces;
using Tawreed.Domain.Enums;
using Tawreed.Domain.Interfaces;

namespace Tawreed.Application.Services;

public class BuyerProfileService : IBuyerProfileService
{
    private readonly IBuyerRepository _buyerRepository;
    private readonly IRegionRepository _regionRepository;
    private readonly IAppSettingRepository _appSettingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BuyerProfileService(
        IBuyerRepository buyerRepository,
        IRegionRepository regionRepository,
        IAppSettingRepository appSettingRepository,
        IUnitOfWork unitOfWork)
    {
        _buyerRepository = buyerRepository;
        _regionRepository = regionRepository;
        _appSettingRepository = appSettingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BuyerProfileDto> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var buyer = await _buyerRepository.GetByUserIdWithDetailsAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("Buyer profile not found.");

        var dto = new BuyerProfileDto
        {
            Name = buyer.User?.FullName ?? "",
            Email = buyer.User?.Email ?? "",
            Phone = buyer.User?.Phone ?? "",
            BusinessName = buyer.BusinessName,
            BusinessType = buyer.BusinessType,
            TaxId = buyer.TaxId,
            CommercialRegistrationNo = buyer.CommercialRegistrationNo,
            Avatar = buyer.User?.AvatarUrl,
            JoinedDate = buyer.CreatedAt,
            RegionName = buyer.Region?.NameEn ?? "",
            RegionId = buyer.RegionId,
            PreferredLang = buyer.User?.PreferredLang ?? "en"
        };

        // Resolve the group region (ancestor matching allowed types, e.g. Markaz)
        // Start from the buyer's parent — the buyer's own region (e.g. village/kafr)
        // should never be the group region; we always walk up at least one level.
        var allowedTypes = await GetAllowedGroupRegionTypesAsync(cancellationToken);
        if (allowedTypes.Count > 0 && buyer.Region?.ParentId != null)
        {
            var groupRegion = await _regionRepository.FindMatchingAncestorAsync(buyer.Region.ParentId.Value, allowedTypes, cancellationToken);
            if (groupRegion != null)
            {
                dto.GroupRegionId = groupRegion.Id;
                dto.GroupRegionNameAr = groupRegion.NameAr;
                dto.GroupRegionNameEn = groupRegion.NameEn;
            }
        }
        else if (allowedTypes.Count > 0 && buyer.Region?.ParentId == null)
        {
            // Buyer is at root level — use as-is
            dto.GroupRegionId = buyer.RegionId;
            dto.GroupRegionNameAr = buyer.Region?.NameAr ?? "";
            dto.GroupRegionNameEn = buyer.Region?.NameEn ?? "";
        }
        else
        {
            // No configured group region types — use the buyer's own region
            dto.GroupRegionId = buyer.RegionId;
            dto.GroupRegionNameAr = buyer.Region?.NameAr ?? "";
            dto.GroupRegionNameEn = buyer.Region?.NameEn ?? "";
        }

        return dto;
    }

    private async Task<List<RegionType>> GetAllowedGroupRegionTypesAsync(CancellationToken cancellationToken)
    {
        var setting = await _appSettingRepository.GetByKeyAsync("GroupRegionTypes", cancellationToken);
        if (setting is null || string.IsNullOrWhiteSpace(setting.Value))
            return [];

        try
        {
            var typeNames = JsonSerializer.Deserialize<List<string>>(setting.Value) ?? [];
            return typeNames
                .Select(n => Enum.TryParse<RegionType>(n, true, out var t) ? t : (RegionType?)null)
                .Where(t => t.HasValue)
                .Select(t => t!.Value)
                .ToList();
        }
        catch
        {
            return [];
        }
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
        if (request.CommercialRegistrationNo != null) buyer.CommercialRegistrationNo = request.CommercialRegistrationNo;
        if (request.RegionId.HasValue) buyer.RegionId = request.RegionId.Value;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
