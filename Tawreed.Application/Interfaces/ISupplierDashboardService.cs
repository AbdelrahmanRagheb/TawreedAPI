namespace Tawreed.Application.Interfaces;

public class SupplierDashboardData
{
    public required SupplierKpiDto Kpi { get; set; }
    public required IReadOnlyList<SupplierPendingOrderDto> PendingOrders { get; set; }
    public required IReadOnlyList<SupplierActiveGroupOrderDto> ActiveGroupOrders { get; set; }
    public required IReadOnlyList<InventoryAlertDto> InventoryAlerts { get; set; }
    public required DeliveryOverviewDto DeliveryOverview { get; set; }
    public required IReadOnlyList<RecentActivityDto> RecentActivity { get; set; }
}

public class SupplierKpiDto
{
    public decimal TotalRevenue { get; set; }
    public int TotalOrders { get; set; }
    public int ActiveOrders { get; set; }
    public int PendingDeliveries { get; set; }
    public int TotalProducts { get; set; }
    public decimal RatingAvg { get; set; }
}

public class SupplierPendingOrderDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string CreatorName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTimeOffset Deadline { get; set; }
    public string Region { get; set; } = string.Empty;
    public DateTimeOffset ReceivedAt { get; set; }
}

public class SupplierActiveGroupOrderDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Participants { get; set; }
    public decimal TotalValue { get; set; }
    public DateTimeOffset Deadline { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class InventoryAlertDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Stock { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class DeliveryOverviewDto
{
    public int Pending { get; set; }
    public int Preparing { get; set; }
    public int Shipped { get; set; }
    public int Delivered { get; set; }
    public int Failed { get; set; }
}

public class RecentActivityDto
{
    public string Action { get; set; } = string.Empty;
    public DateTimeOffset Time { get; set; }
}

public interface ISupplierDashboardService
{
    Task<SupplierDashboardData> GetDashboardAsync(Guid userId, CancellationToken cancellationToken = default);
}
