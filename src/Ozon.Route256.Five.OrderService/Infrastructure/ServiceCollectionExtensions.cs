using FluentValidation;
using Ozon.Route256.Five.LogisticsSimulator.Grpc;
using Ozon.Route256.Five.OrderService.ClientBalancing;
using Ozon.Route256.Five.OrderService.Core;
using Ozon.Route256.Five.OrderService.Core.Handlers.AggregateOrders;
using Ozon.Route256.Five.OrderService.Core.Handlers.CustomersGet;
using Ozon.Route256.Five.OrderService.Core.Handlers.OrderCancel;
using Ozon.Route256.Five.OrderService.Core.Handlers.OrderGet;
using Ozon.Route256.Five.OrderService.Core.Handlers.OrdersByCustomerGet;
using Ozon.Route256.Five.OrderService.Core.Handlers.OrdersGet;
using Ozon.Route256.Five.OrderService.Core.Handlers.OrderStatusGet;
using Ozon.Route256.Five.OrderService.Core.Handlers.RegionsGet;
using Ozon.Route256.Five.OrderService.Core.Redis;
using Ozon.Route256.Five.OrderService.Core.Redis.Settings;
using Ozon.Route256.Five.OrderService.Core.Repository;
using Ozon.Route256.Five.OrderService.Core.Repository.Imp;
using Ozon.Route256.Five.OrderService.Db;
using Ozon.Route256.Five.OrderService.Db.Repositories;
using Ozon.Route256.Five.OrderService.Grpc;
using Ozon.Route256.Five.OrderService.Kafka;
using Ozon.Route256.Five.OrderService.Rest.Dto;
using Ozon.Route256.Five.OrderService.Shared.ClientBalancing;
using Ozon.Route256.Five.OrderService.Validators;

namespace Ozon.Route256.Five.OrderService.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection SetupGrpc(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddGrpc(options => { options.Interceptors.Add<LoggerInterceptor>(); });
        services.AddGrpcReflection();

        services.AddSingleton<LoggerInterceptor>();
        services.AddScoped<ILogisticService, LogisticService>();
        services.AddScoped<ICustomerService, CustomerService>();

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

        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHandlers()
            .AddRepositories()
            .AddHostedService<SdConsumerHostedService>()
            .AddRedis(configuration)
            .AddKafka(configuration)
            .AddSwaggerGen()
            .AddMigrations(configuration);

        services.AddSingleton<IDbStore, DbStore>();

        services.AddScoped<IValidator<OrdersByCustomerRequestDto>, OrdersByCustomerRequestDtoValidator>();
        services.AddScoped<IValidator<AggregatedOrdersRequestDto>, AggregatedOrdersRequestDtoValidator>();

        services.Configure<PostgresSettings>(configuration.GetSection("Postgres"));

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IOrderRepository, OrderDbRepository>();
        services.AddScoped<IRegionRepository, RegionDbRepository>();
        services.AddScoped<ICustomerRepository, CustomerRedisRepository>();

        return services;
    }

    public static IServiceCollection AddHandlers(this IServiceCollection collection)
    {
        collection.AddScoped<IOrderAggregationHandler, OrderAggregationHandler>();
        collection.AddScoped<ICustomersGettingHandler, CustomersGettingHandler>();
        collection.AddScoped<IOrderCancellationHandler, OrderCancellationHandler>();
        collection.AddScoped<IOrderGettingHandler, OrderGettingHandler>();
        collection.AddScoped<IOrdersByCustomerGettingHandler, OrdersByCustomerGettingHandler>();
        collection.AddScoped<IOrdersGettingHandler, OrdersGettingHandler>();
        collection.AddScoped<IOrderStatusGettingHandler, OrderStatusGettingHandler>();
        collection.AddScoped<IRegionsGettingHandler, RegionsGettingHandler>();

        return collection;
    }
}