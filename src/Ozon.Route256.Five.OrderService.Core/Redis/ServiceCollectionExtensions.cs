using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ozon.Route256.Five.OrderService.Core.Redis.Settings;

namespace Ozon.Route256.Five.OrderService.Core.Redis;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RedisSettings>(configuration.GetSection("Redis"));

        services.AddSingleton<IRedisDatabaseAccessor, RedisDatabaseAccessor>();

        return services;
    }
}