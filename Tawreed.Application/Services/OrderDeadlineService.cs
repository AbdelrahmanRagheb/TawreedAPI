using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tawreed.Domain.Entities;
using Tawreed.Domain.Enums;
using Tawreed.Domain.Interfaces;

namespace Tawreed.Application.Services;

internal sealed class OrderDeadlineService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OrderDeadlineService> _logger;

    public OrderDeadlineService(IServiceScopeFactory scopeFactory, ILogger<OrderDeadlineService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OrderDeadlineService started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IGroupOrderRepository>();
                var eventRepo = scope.ServiceProvider.GetRequiredService<IGroupOrderEventRepository>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var openOrders = await repo.GetByStatusAsync(OrderStatus.Open, stoppingToken);
                var now = DateTimeOffset.UtcNow;
                var expired = openOrders.Where(o => o.DeadlineAt <= now).ToList();

                foreach (var order in expired)
                {
                    order.Status = OrderStatus.Closed;
                    order.ClosedAt = now;
                    repo.Update(order);

                    eventRepo.Add(new GroupOrderEvent
                    {
                        Id = Guid.NewGuid(),
                        GroupOrderId = order.Id,
                        EventType = "Closed",
                        NotesEn = "Auto-closed after deadline passed",
                        NotesAr = "تم الإغلاق تلقائياً بعد انتهاء المهلة",
                        CreatedBy = Guid.Empty
                    });
                }

                if (expired.Count > 0)
                {
                    await unitOfWork.SaveChangesAsync(stoppingToken);
                    _logger.LogInformation("Closed {Count} expired orders.", expired.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking order deadlines.");
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        _logger.LogInformation("OrderDeadlineService stopped.");
    }
}
