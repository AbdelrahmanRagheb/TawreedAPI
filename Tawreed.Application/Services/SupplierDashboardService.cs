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

        var allOrders = await _groupOrderRepository.GetAllAsync(cancellationToken);
        var ordersWithMyItems = allOrders
            .Where(o => o.Items != null && o.Items.Any(i => i.SupplierId == supplier.Id))
            .ToList();
        var products = await _supplierProductRepository.GetBySupplierAsync(supplier.Id, cancellationToken);
        var deliveries = await _deliveryRepository.GetAllAsync(cancellationToken);
        var supplierDeliveries = deliveries.Where(d => d.SupplierId == supplier.Id).ToList();

        var totalRevenue = ordersWithMyItems
            .Where(o => o.Status == OrderStatus.Completed)
            .SelectMany(o => o.Items ?? [])
            .Where(i => i.SupplierId == supplier.Id)
            .Sum(i => (i.UnitPrice ?? 0) * (i.ParticipantItems?.Sum(pi => pi.Quantity) ?? 0));

        return new SupplierDashboardData
        {
            Kpi = new SupplierKpiDto
            {
                TotalRevenue = totalRevenue,
                TotalOrders = ordersWithMyItems.Count,
                ActiveOrders = ordersWithMyItems.Count(o => o.Status == OrderStatus.Open),
                PendingDeliveries = supplierDeliveries.Count(d => d.Status is "Pending" or "Preparing"),
                TotalProducts = products.Count(p => p.IsActive)
            },
            PendingOrders = ordersWithMyItems
                .Where(o => o.Items!.Any(i => i.SupplierId == supplier.Id && i.ItemStatus == "Pending"))
                .Select(o => new SupplierPendingOrderDto
                {
                    Id = o.Id,
                    Title = o.Title,
                    CreatorName = o.Creator?.User?.FullName ?? "",
                    Participants = o.Participants?.Count(p => p.Status == "Joined") ?? 0,
                    TotalAmount = o.Items?.Sum(i => (i.UnitPrice ?? 0) * i.TargetQty) ?? 0,
                    Deadline = o.DeadlineAt,
                    Region = o.Region?.NameEn ?? "",
                    ReceivedAt = o.CreatedAt
                })
                .ToList(),
            ActiveGroupOrders = allOrders
                .Where(o => o.Status is OrderStatus.Open or OrderStatus.PendingApproval or OrderStatus.Locked)
                .Select(o => new SupplierActiveGroupOrderDto
                {
                    Id = o.Id,
                    Title = o.Title,
                    Participants = o.Participants?.Count(p => p.Status == "Joined") ?? 0,
                    TotalValue = o.Items?.Sum(i => (i.UnitPrice ?? 0) * i.TargetQty) ?? 0,
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
                .SelectMany(o => o.Events!.Select(e => new { Order = o, Event = e }))
                .Where(x => x.Event.EventType is "SupplierAssigned" or "SupplierApproved" or "SupplierDeclined" or "Closed" or "Completed")
                .OrderByDescending(x => x.Event.CreatedAt)
                .Take(10)
                .Select(x => new RecentActivityDto
                {
                    ActionEn = x.Event.EventType switch
                    {
                        "SupplierAssigned" => $"Order '{x.Order.Title}' — requires your action (accept or decline)",
                        "SupplierApproved" => "You accepted the order",
                        "SupplierDeclined" => "You declined the order",
                        "Closed" => "Order closed automatically",
                        "Completed" => "Order completed",
                        _ => x.Event.NotesEn ?? x.Event.EventType
                    },
                    ActionAr = x.Event.EventType switch
                    {
                        "SupplierAssigned" => $"الطلب '{x.Order.Title}' — يتطلب موافقتك (قبول أو رفض)",
                        "SupplierApproved" => "قبلت الطلب",
                        "SupplierDeclined" => "رفضت الطلب",
                        "Closed" => "أُغلق الطلب تلقائياً",
                        "Completed" => "اكتمل الطلب",
                        _ => x.Event.NotesAr ?? x.Event.EventType
                    },
                    Time = x.Event.CreatedAt
                })
                .ToList()
        };
    }
}
