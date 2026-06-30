using System.Text.Json;
using Tawreed.Application.Common.Models;
using Tawreed.Application.Interfaces;
using Tawreed.Domain.Entities;
using Tawreed.Domain.Enums;
using Tawreed.Domain.Interfaces;

namespace Tawreed.Application.Services;

public class GroupOrderService : IGroupOrderService
{
    private readonly IGroupOrderRepository _groupOrderRepository;
    private readonly IAppSettingRepository _appSettingRepository;
    private readonly IRegionRepository _regionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GroupOrderService(
        IGroupOrderRepository groupOrderRepository,
        IAppSettingRepository appSettingRepository,
        IRegionRepository regionRepository,
        IUnitOfWork unitOfWork)
    {
        _groupOrderRepository = groupOrderRepository;
        _appSettingRepository = appSettingRepository;
        _regionRepository = regionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<GroupOrderDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var orders = await _groupOrderRepository.GetAllAsync(cancellationToken);
        return orders.Select(MapToDto).ToList();
    }

    public async Task<GroupOrderDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _groupOrderRepository.GetWithDetailsAsync(id, cancellationToken);
        return order is null ? null : MapToDto(order);
    }

    public async Task<GroupOrderDto> CreateAsync(GroupOrder groupOrder, CancellationToken cancellationToken = default)
    {
        var region = await _regionRepository.GetByIdAsync(groupOrder.RegionId, cancellationToken)
            ?? throw new KeyNotFoundException($"Region ({groupOrder.RegionId}) not found.");

        var allowedTypes = await GetAllowedGroupRegionTypesAsync(cancellationToken);
        if (allowedTypes.Count > 0)
        {
            var matched = await _regionRepository.FindMatchingAncestorAsync(
                groupOrder.RegionId, allowedTypes, cancellationToken);

            if (matched is null)
                throw new InvalidOperationException(
                    $"Region '{region.NameEn}' does not match any allowed group region type. " +
                    $"Allowed types: {string.Join(", ", allowedTypes.Select(t => t.ToString()))}");

            groupOrder.RegionId = matched.Id;
        }

        _groupOrderRepository.Add(groupOrder);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(groupOrder);
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

    public async Task<GroupOrderDto> UpdateStatusAsync(Guid id, string status, CancellationToken cancellationToken = default)
    {
        var order = await _groupOrderRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException($"Group order ({id}) not found.");

        if (order.Status == OrderStatus.Closed && status != OrderStatus.Closed)
            throw new InvalidOperationException("Cannot reopen a closed order.");

        order.Status = status;
        if (status is OrderStatus.Completed or OrderStatus.Cancelled)
            order.ClosedAt = DateTimeOffset.UtcNow;

        _groupOrderRepository.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return MapToDto(order);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _groupOrderRepository.GetByIdAsync(id, cancellationToken);
        if (order is not null)
        {
            _groupOrderRepository.Delete(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    private static GroupOrderDto MapToDto(GroupOrder o) =>
        new(o.Id, o.CreatorId, o.SupplierId, o.RegionId, o.Title, o.Description,
            o.OrderNumber, o.DeadlineAt, o.Status,
            o.ClosedAt, o.CreatedAt, o.UpdatedAt);
}
