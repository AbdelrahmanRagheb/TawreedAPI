using Microsoft.EntityFrameworkCore;
using Tawreed.Application.Common.Models;
using Tawreed.Application.Interfaces;
using Tawreed.Domain.Enums;
using Tawreed.Domain.Interfaces;

namespace Tawreed.Application.Services;

public class BuyerDashboardService : IBuyerDashboardService
{
    private readonly IGroupOrderRepository _groupOrderRepository;
    private readonly IGroupOrderParticipantRepository _participantRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly ISupplierProductRepository _supplierProductRepository;
    private readonly IBuyerRepository _buyerRepository;
    private readonly IRegionRepository _regionRepository;

    public BuyerDashboardService(
        IGroupOrderRepository groupOrderRepository,
        IGroupOrderParticipantRepository participantRepository,
        INotificationRepository notificationRepository,
        ISupplierProductRepository supplierProductRepository,
        IBuyerRepository buyerRepository,
        IRegionRepository regionRepository)
    {
        _groupOrderRepository = groupOrderRepository;
        _participantRepository = participantRepository;
        _notificationRepository = notificationRepository;
        _supplierProductRepository = supplierProductRepository;
        _buyerRepository = buyerRepository;
        _regionRepository = regionRepository;
    }

    public async Task<BuyerDashboardData> GetDashboardAsync(Guid userId, Guid? regionId = null, CancellationToken cancellationToken = default)
    {
        var buyer = await _buyerRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("Buyer profile not found.");

        var myParticipations = await _participantRepository.GetByBuyerAsync(buyer.Id, cancellationToken);
        var myOrderIds = myParticipations.Select(p => p.GroupOrderId).ToHashSet();

        var regionIds = await _regionRepository.GetAncestorIdsAsync(buyer.RegionId, cancellationToken);
        regionIds.Add(buyer.RegionId);
        var regionSet = regionIds.ToHashSet();

        var allOrders = await _groupOrderRepository.GetAllWithAdminDetailsAsync(cancellationToken);
        var myOrders = allOrders.Where(o => o.CreatorId == buyer.Id || myOrderIds.Contains(o.Id)).ToList();

        var activeOrders = myOrders
            .Where(o => o.Status == OrderStatus.Open)
            .Select(o =>
            {
                var participants = o.Participants?.Where(p => p.Status == "Joined").ToList() ?? [];
                return new ActiveOrderDto
                {
                    Id = o.Id,
                    Title = o.Title,
                    Status = o.Status,
                    Deadline = o.DeadlineAt,
                    ParticipantCount = participants.Count,
                    TotalValue = o.Items?.Sum(i => (i.UnitPrice ?? 0) * i.TargetQty) ?? 0,
                    ProductCount = o.Items?.Count ?? 0,
                    CreatorName = o.Creator?.User?.FullName ?? "",
                    Region = o.Region?.NameEn ?? "",
                    IsCreator = o.CreatorId == buyer.Id
                };
            })
            .ToList();

        var nearbyOrders = allOrders
            .Where(o => o.Status == OrderStatus.Open && regionSet.Contains(o.RegionId) && o.CreatorId != buyer.Id && !myOrderIds.Contains(o.Id))
            .Select(o =>
            {
                var product = o.Items?.FirstOrDefault();
                return new NearbyOrderDto
                {
                    Id = o.Id,
                    Title = o.Title,
                    CreatorName = o.Creator?.User?.FullName ?? "",
                    ProductName = product?.Product?.Name ?? "",
                    Quantity = product?.TargetQty ?? 0,
                    CurrentParticipants = o.Participants?.Count(p => p.Status == "Joined") ?? 0,
                    Deadline = o.DeadlineAt,
                    Region = o.Region?.NameEn ?? ""
                };
            })
            .Take(10)
            .ToList();

        var notifications = await _notificationRepository.GetUnreadByUserAsync(userId, cancellationToken);
        var unreadCount = await _notificationRepository.GetUnreadCountAsync(userId, cancellationToken);

        var allProducts = await _supplierProductRepository.GetAllAsync(cancellationToken);
        var trending = allProducts
            .Where(p => p.IsActive)
            .Select(p => new TrendingProductDto
            {
                Id = p.ProductId,
                Name = p.Product?.Name ?? "",
                Price = p.Price,
                ImageUrl = p.Images?.FirstOrDefault(i => i.IsCover)?.ImageUrl ?? p.Images?.FirstOrDefault()?.ImageUrl,
                CategoryName = p.Product?.Category?.NameAr ?? "",
                OrderCount = p.GroupOrderItems?.Count ?? 0
            })
            .OrderByDescending(p => p.OrderCount)
            .Take(10)
            .ToList();

        return new BuyerDashboardData
        {
            ActiveOrders = activeOrders,
            NearbyOrders = nearbyOrders,
            Notifications = notifications.Select(n => new NotificationDto(
                n.Id,
                n.UserId,
                n.Type,
                n.TitleAr,
                n.TitleEn,
                n.BodyAr,
                n.BodyEn,
                n.Channel,
                n.IsRead,
                n.ReadAt,
                n.RelatedOrderId,
                n.CreatedAt
            )).ToList(),
            TrendingProducts = trending,
            TotalSavings = 0,
            UnreadNotificationCount = unreadCount
        };
    }
}
