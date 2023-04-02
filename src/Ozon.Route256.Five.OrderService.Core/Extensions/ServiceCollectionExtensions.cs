using Microsoft.Extensions.DependencyInjection;
using Ozon.Route256.Five.OrderService.Core.ClientBalancing;
using Ozon.Route256.Five.OrderService.Core.Handlers.AggregateOrders;
using Ozon.Route256.Five.OrderService.Core.Handlers.CustomersGet;
using Ozon.Route256.Five.OrderService.Core.Handlers.OrderCancel;
using Ozon.Route256.Five.OrderService.Core.Handlers.OrderGet;
using Ozon.Route256.Five.OrderService.Core.Handlers.OrdersByCustomerGet;
using Ozon.Route256.Five.OrderService.Core.Handlers.OrdersGet;
using Ozon.Route256.Five.OrderService.Core.Handlers.OrderStatusGet;
using Ozon.Route256.Five.OrderService.Core.Handlers.RegionsGet;

namespace Ozon.Route256.Five.OrderService.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        services.AddScoped<IOrderAggregationHandler, OrderAggregationHandler>();
        services.AddScoped<ICustomersGettingHandler, CustomersGettingHandler>();
        services.AddScoped<IOrderCancellationHandler, OrderCancellationHandler>();
        services.AddScoped<IOrderGettingHandler, OrderGettingHandler>();
        services.AddScoped<IOrdersByCustomerGettingHandler, OrdersByCustomerGettingHandler>();
        services.AddScoped<IOrdersGettingHandler, OrdersGettingHandler>();
        services.AddScoped<IOrderStatusGettingHandler, OrderStatusGettingHandler>();
        services.AddScoped<IRegionsGettingHandler, RegionsGettingHandler>();

        return services;
    }

    public static IServiceCollection AddDbStore(this IServiceCollection services)
    {
        services.AddSingleton<IDbStore, DbStore>();

        return services;
    }
}