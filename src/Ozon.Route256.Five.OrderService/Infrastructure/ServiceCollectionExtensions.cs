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
using Ozon.Route256.Five.OrderService.Core.Repository;
using Ozon.Route256.Five.OrderService.Core.Repository.Imp;
using Ozon.Route256.Five.OrderService.Grpc;
using Ozon.Route256.Five.OrderService.Kafka;
using Ozon.Route256.Five.OrderService.Rest.Dto;
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

        services.AddGrpcClient<SdService.SdServiceClient>(
                options =>
                {
                    const string KEY = "ROUTE256_SERVICE_DISCOVERY_ADDRESS";
                    string? address = configuration.GetValue<string>(KEY);
                    if (string.IsNullOrWhiteSpace(address))
                    {
                        throw new NullReferenceException(KEY);
                    }
                    options.Address = new Uri(address);
                })
            .AddInterceptor<LoggerInterceptor>();

        services.AddGrpcClient<LogisticsSimulatorService.LogisticsSimulatorServiceClient>(
                options =>
                {
                    const string KEY = "ROUTE256_LOGISTICS_SIMULATOR_ADDRESS";
                    string? address = configuration.GetValue<string>(KEY);
                    if (string.IsNullOrWhiteSpace(address))
                    {
                        throw new NullReferenceException(KEY);
                    }
                    options.Address = new Uri(address);
                })
            .AddInterceptor<LoggerInterceptor>();

        services.AddGrpcClient<Customers.CustomersClient>(
                options =>
                {
                    const string KEY = "ROUTE256_CUSTOMER_SERVICE_ADDRESS";
                    string? address = configuration.GetValue<string>(KEY);
                    if (string.IsNullOrWhiteSpace(address))
                    {
                        throw new NullReferenceException(KEY);
                    }
                    options.Address = new Uri(address);
                })
            .AddInterceptor<LoggerInterceptor>();

        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        const string KEY = "Redis:ConnectionString";
        string connectionString = configuration.GetValue<string>(KEY);
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new NullReferenceException(KEY);
        }

        services
            .AddHandlers()
            .AddRepositories()
            .AddHostedService<SdConsumerHostedService>()
            .AddRedis(connectionString)
            .AddKafka(configuration)
            .AddSwaggerGen();

        services.AddSingleton<IDbStore, DbStore>();

        services.AddScoped<IValidator<OrdersByCustomerRequestDto>, OrdersByCustomerRequestDtoValidator>();
        services.AddScoped<IValidator<AggregatedOrdersRequestDto>, AggregatedOrdersRequestDtoValidator>();

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<InMemoryStorage>();

        services.AddScoped<IOrderRepository, OrderRedisRepository>();
        services.AddScoped<IRegionRepository, RegionInMemoryRepository>();
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