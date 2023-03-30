using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ozon.Route256.Five.OrderService.Db.Repositories;

namespace Ozon.Route256.Five.OrderService.Db;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AdDb(this IServiceCollection services, IConfiguration configuration)
    {
        SqlMapper.AddTypeHandler(new WarehouseDtoTypeMapper());

        services.AddScoped<IConnectionFactory, ConnectionFactory>();
        services.AddScoped<IShardingRule<long>, RoundRobinLongShardingRule>();
        services.AddScoped<IShardingRule<string>, RoundRobinStringShardingRule>();

        return services;
    }
}