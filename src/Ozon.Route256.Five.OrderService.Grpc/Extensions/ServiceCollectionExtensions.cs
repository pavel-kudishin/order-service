using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Ozon.Route256.Five.LogisticsSimulator.Grpc;
using Ozon.Route256.Five.OrderService.Core.Abstractions;
using Ozon.Route256.Five.OrderService.Core.Exceptions;
using Ozon.Route256.Five.OrderService.Core.Extensions;
using Ozon.Route256.Five.OrderService.Grpc.GrpcServices;
using Ozon.Route256.Five.OrderService.Grpc.Logging;
using Ozon.Route256.Five.OrderService.Grpc.Services;

namespace Ozon.Route256.Five.OrderService.Grpc.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection SetupGrpc(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc(options =>
        {
            options.Interceptors.Add<LoggerInterceptor>();
            options.Interceptors.Add<TraceInterceptor>();
            options.Interceptors.Add<MetricsInterceptor>();
        });
        services.AddGrpcReflection();

        services.AddSingleton<LoggerInterceptor>();
        services.TryAddSingleton<IGrpcMetrics, GrpcMetrics>();

        string serviceDiscoveryAddress =
            configuration.GetValue<string>(Constants.SERVICE_DISCOVERY_ADDRESS_KEY)
            ?? throw new InvalidConfigurationException(Constants.SERVICE_DISCOVERY_ADDRESS_KEY);

        string logisticsSimulatorAddress =
            configuration.GetValue<string>(Constants.LOGISTICS_SIMULATOR_ADDRESS_KEY)
            ?? throw new InvalidConfigurationException(Constants.LOGISTICS_SIMULATOR_ADDRESS_KEY);

        string customerServiceAddress =
            configuration.GetValue<string>(Constants.CUSTOMER_SERVICE_ADDRESS_KEY)
            ?? throw new InvalidConfigurationException(Constants.CUSTOMER_SERVICE_ADDRESS_KEY);

        services.AddGrpcClient<SdService.SdServiceClient>(
                options =>
                {
                    options.Address = new Uri(serviceDiscoveryAddress);
                })
            .AddInterceptor<LoggerInterceptor>();

        services.AddGrpcClient<LogisticsSimulatorService.LogisticsSimulatorServiceClient>(
                options =>
                {
                    options.Address = new Uri(logisticsSimulatorAddress);
                })
            .AddInterceptor<LoggerInterceptor>();

        services.AddGrpcClient<Customers.CustomersClient>(
                options =>
                {
                    options.Address = new Uri(customerServiceAddress);
                })
            .AddInterceptor<LoggerInterceptor>();

        services.AddScoped<ILogisticService, LogisticService>();
        services.AddScoped<ICustomerService, CustomerService>();

        return services;
    }

    public static void MapGrpc(this IEndpointRouteBuilder routeBuilder)
    {
        routeBuilder.MapGrpcService<OrdersService>();
        routeBuilder.MapGrpcReflectionService();
    }
}