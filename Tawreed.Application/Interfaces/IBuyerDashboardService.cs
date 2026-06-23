using Tawreed.Domain.Entities;

namespace Tawreed.Application.Interfaces;

public class BuyerDashboardData
{
    public required IReadOnlyList<ActiveOrderDto> ActiveOrders { get; set; }
    public required IReadOnlyList<NearbyOrderDto> NearbyOrders { get; set; }
    public required IReadOnlyList<Notification> Notifications { get; set; }
    public required IReadOnlyList<TrendingProductDto> TrendingProducts { get; set; }
    public decimal TotalSavings { get; set; }
    public int UnreadNotificationCount { get; set; }
}

public class ActiveOrderDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset Deadline { get; set; }
    public int ParticipantCount { get; set; }
    public decimal TotalValue { get; set; }
    public int ProductCount { get; set; }
    public string CreatorName { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public bool IsCreator { get; set; }
}

public class NearbyOrderDto
{
    public Guid Id { get; set; }
    public string CreatorName { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int CurrentParticipants { get; set; }
    public DateTimeOffset Deadline { get; set; }
    public string Region { get; set; } = string.Empty;
}

public class TrendingProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int OrderCount { get; set; }
}

public interface IBuyerDashboardService
{
    Task<BuyerDashboardData> GetDashboardAsync(Guid userId, Guid? regionId = null, CancellationToken cancellationToken = default);
}
