using Microsoft.Extensions.DependencyInjection;
using Tawreed.Application.Interfaces;
using Tawreed.Application.Services;

namespace Tawreed.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ISupplierProductService, SupplierProductService>();
        services.AddScoped<IPricingTierService, PricingTierService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IGroupOrderService, GroupOrderService>();
        services.AddScoped<IRegionService, RegionService>();
        services.AddScoped<INotificationService, NotificationService>();

        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IBuyerDashboardService, BuyerDashboardService>();
        services.AddScoped<IBuyerOrderService, BuyerOrderService>();
        services.AddScoped<ISupplierDashboardService, SupplierDashboardService>();
        services.AddScoped<ISupplierOrderService, SupplierOrderService>();
        services.AddScoped<ISupplierProfileService, SupplierProfileService>();
        services.AddScoped<IBuyerProfileService, BuyerProfileService>();
        services.AddScoped<IBuyerSupplierService, BuyerSupplierService>();
        services.AddScoped<IAdminService, AdminService>();

        services.AddHostedService<OrderDeadlineService>();

        return services;
    }
}
