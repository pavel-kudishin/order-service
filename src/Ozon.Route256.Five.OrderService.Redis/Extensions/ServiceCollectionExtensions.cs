using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ozon.Route256.Five.OrderService.Domain.Repository;
using Ozon.Route256.Five.OrderService.Redis.Repository;
using Ozon.Route256.Five.OrderService.Redis.Settings;

namespace Ozon.Route256.Five.OrderService.Redis.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RedisSettings>(configuration.GetSection("Redis"));

        services.AddSingleton<IRedisDatabaseAccessor, RedisDatabaseAccessor>();
        services.AddScoped<ICustomerRepository, CustomerRedisRepository>();

        return services;
    }
}