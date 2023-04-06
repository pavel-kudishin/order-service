using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ozon.Route256.Five.OrderService.Db.Repositories;
using Ozon.Route256.Five.OrderService.Db.Repositories.Harness;
using Ozon.Route256.Five.OrderService.Db.Settings;
using Ozon.Route256.Five.OrderService.Domain.Repository;

namespace Ozon.Route256.Five.OrderService.Db.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDb(this IServiceCollection services, IConfiguration configuration)
    {
        SqlMapper.AddTypeHandler(new WarehouseDtoTypeMapper());

        services.Configure<PostgresSettings>(configuration.GetSection("Postgres"));
        services.AddScoped<IConnectionFactory, ConnectionFactory>();
        services.AddShardingRules();
        services.AddRepositories();

        return services;
    }

    public static void AddShardingRules(this IServiceCollection services)
    {
        services.AddScoped<IShardingRule<long>, RoundRobinLongShardingRule>();
        services.AddScoped<IShardingRule<string>, RoundRobinStringShardingRule>();
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IOrderRepository, OrderDbRepository>();
        services.AddScoped<IRegionRepository, RegionDbRepository>();

        return services;
    }
}