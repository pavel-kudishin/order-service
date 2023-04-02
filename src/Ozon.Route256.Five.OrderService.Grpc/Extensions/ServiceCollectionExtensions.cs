using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ozon.Route256.Five.LogisticsSimulator.Grpc;
using Ozon.Route256.Five.OrderService.Core.Abstractions;
using Ozon.Route256.Five.OrderService.Core.Exceptions;
using Ozon.Route256.Five.OrderService.Grpc.GrpcServices;
using Ozon.Route256.Five.OrderService.Grpc.Services;

namespace Ozon.Route256.Five.OrderService.Grpc.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection SetupGrpc(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc(options =>
        {
            options.Interceptors.Add<LoggerInterceptor>();
        });
        services.AddGrpcReflection();

        services.AddSingleton<LoggerInterceptor>();

        const string SERVICE_DISCOVERY_ADDRESS_KEY = "ROUTE256_SERVICE_DISCOVERY_ADDRESS";
        string serviceDiscoveryAddress =
            configuration.GetValue<string>(SERVICE_DISCOVERY_ADDRESS_KEY)
            ?? throw new InvalidConfigurationException(SERVICE_DISCOVERY_ADDRESS_KEY);

        const string LOGISTICS_SIMULATOR_ADDRESS_KEY = "ROUTE256_LOGISTICS_SIMULATOR_ADDRESS";
        string logisticsSimulatorAddress =
            configuration.GetValue<string>(LOGISTICS_SIMULATOR_ADDRESS_KEY)
            ?? throw new InvalidConfigurationException(LOGISTICS_SIMULATOR_ADDRESS_KEY);

        const string CUSTOMER_SERVICE_ADDRESS_KEY = "ROUTE256_CUSTOMER_SERVICE_ADDRESS";
        string customerServiceAddress =
            configuration.GetValue<string>(CUSTOMER_SERVICE_ADDRESS_KEY)
            ?? throw new InvalidConfigurationException(CUSTOMER_SERVICE_ADDRESS_KEY);

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