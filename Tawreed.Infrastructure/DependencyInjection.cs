using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tawreed.Domain.Interfaces;
using Tawreed.Infrastructure.Data;
using Tawreed.Infrastructure.Repositories;
using Tawreed.Infrastructure.Services;

namespace Tawreed.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b =>
                {
                    b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                    b.UseNetTopologySuite();
                })
            .UseSnakeCaseNamingConvention());

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRegionRepository, RegionRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IUnitRepository, UnitRepository>();
        services.AddScoped<IBuyerRepository, BuyerRepository>();
        services.AddScoped<ISupplierRepository, SupplierRepository>();
        services.AddScoped<ISupplierApprovalLogRepository, SupplierApprovalLogRepository>();
        services.AddScoped<ISupplierCategoryRepository, SupplierCategoryRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ISupplierProductRepository, SupplierProductRepository>();
        services.AddScoped<IProductImageRepository, ProductImageRepository>();
        services.AddScoped<IPricingTierRepository, PricingTierRepository>();
        services.AddScoped<IGroupOrderRepository, GroupOrderRepository>();
        services.AddScoped<IGroupOrderItemRepository, GroupOrderItemRepository>();
        services.AddScoped<IGroupOrderParticipantRepository, GroupOrderParticipantRepository>();
        services.AddScoped<IParticipantItemRepository, ParticipantItemRepository>();
        services.AddScoped<IGroupOrderEventRepository, GroupOrderEventRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IDeliveryRepository, DeliveryRepository>();
        services.AddScoped<IDeliveryPersonProfileRepository, DeliveryPersonProfileRepository>();
        services.AddScoped<IDeliveryAssignmentRequestRepository, DeliveryAssignmentRequestRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IAppSettingRepository, AppSettingRepository>();

        return services;
    }
}
