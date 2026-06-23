using Tawreed.Application.Interfaces;
using Tawreed.Domain.Enums;
using Tawreed.Domain.Interfaces;

namespace Tawreed.Application.Services;

public class SupplierDashboardService : ISupplierDashboardService
{
    private readonly ISupplierRepository _supplierRepository;
    private readonly IGroupOrderRepository _groupOrderRepository;
    private readonly ISupplierProductRepository _supplierProductRepository;
    private readonly IDeliveryRepository _deliveryRepository;

    public SupplierDashboardService(
        ISupplierRepository supplierRepository,
        IGroupOrderRepository groupOrderRepository,
        ISupplierProductRepository supplierProductRepository,
        IDeliveryRepository deliveryRepository)
    {
        _supplierRepository = supplierRepository;
        _groupOrderRepository = groupOrderRepository;
        _supplierProductRepository = supplierProductRepository;
        _deliveryRepository = deliveryRepository;
    }

    public async Task<SupplierDashboardData> GetDashboardAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var supplier = await _supplierRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new KeyNotFoundException("Supplier profile not found.");
        if (!supplier.IsApproved)
            throw new InvalidOperationException("Account pending approval.");

        var allOrders = await _groupOrderRepository.GetBySupplierAsync(supplier.Id, cancellationToken);
        var products = await _supplierProductRepository.GetBySupplierAsync(supplier.Id, cancellationToken);
        var deliveries = await _deliveryRepository.GetAllAsync(cancellationToken);
        var supplierDeliveries = deliveries.Where(d => d.SupplierId == supplier.Id).ToList();

        var totalRevenue = allOrders
            .Where(o => o.Status == OrderStatus.Completed)
            .Sum(o => o.Items?.Sum(i => (i.UnitPrice ?? 0) * (i.ParticipantItems?.Sum(pi => pi.Quantity) ?? 0)) ?? 0);

        return new SupplierDashboardData
        {
            Kpi = new SupplierKpiDto
            {
                TotalRevenue = totalRevenue,
                TotalOrders = allOrders.Count,
                ActiveOrders = allOrders.Count(o => o.Status == OrderStatus.Open),
                PendingDeliveries = supplierDeliveries.Count(d => d.Status is "Pending" or "Preparing"),
                TotalProducts = products.Count(p => p.IsActive),
                RatingAvg = supplier.RatingAvg
            },
            PendingOrders = allOrders
                .Where(o => o.Status == OrderStatus.Open)
                .Select(o => new SupplierPendingOrderDto
                {
                    Id = o.Id,
                    Title = o.Title,
                    CreatorName = o.Creator?.User?.FullName ?? "",
                    TotalAmount = o.Items?.Sum(i => (i.UnitPrice ?? 0) * (i.ParticipantItems?.Sum(pi => pi.Quantity) ?? 0)) ?? 0,
                    Deadline = o.DeadlineAt,
                    Region = o.Region?.NameEn ?? "",
                    ReceivedAt = o.CreatedAt
                })
                .ToList(),
            ActiveGroupOrders = allOrders
                .Where(o => o.Status == OrderStatus.Open)
                .Select(o => new SupplierActiveGroupOrderDto
                {
                    Id = o.Id,
                    Title = o.Title,
                    Participants = o.Participants?.Count(p => p.Status == "Joined") ?? 0,
                    TotalValue = o.Items?.Sum(i => (i.UnitPrice ?? 0) * (i.ParticipantItems?.Sum(pi => pi.Quantity) ?? 0)) ?? 0,
                    Deadline = o.DeadlineAt,
                    Status = o.Status
                })
                .ToList(),
            InventoryAlerts = products
                .Where(p => p.Stock <= 10)
                .Select(p => new InventoryAlertDto
                {
                    ProductId = p.Id,
                    ProductName = p.Product?.Name ?? "",
                    Stock = p.Stock,
                    Status = p.Stock == 0 ? "critical" : "low"
                })
                .ToList(),
            DeliveryOverview = new DeliveryOverviewDto
            {
                Pending = supplierDeliveries.Count(d => d.Status == "Pending"),
                Preparing = supplierDeliveries.Count(d => d.Status == "Preparing"),
                Shipped = supplierDeliveries.Count(d => d.Status == "Shipped"),
                Delivered = supplierDeliveries.Count(d => d.Status == "Delivered"),
                Failed = supplierDeliveries.Count(d => d.Status == "Failed")
            },
            RecentActivity = allOrders
                .Where(o => o.Events != null)
                .SelectMany(o => o.Events!)
                .OrderByDescending(e => e.CreatedAt)
                .Take(10)
                .Select(e => new RecentActivityDto
                {
                    Action = e.EventType,
                    Time = e.CreatedAt
                })
                .ToList()
        };
    }
}
