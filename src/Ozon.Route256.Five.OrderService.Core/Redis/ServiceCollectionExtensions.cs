using Microsoft.Extensions.DependencyInjection;

namespace Ozon.Route256.Five.OrderService.Core.Redis;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRedis(this IServiceCollection services, string? connectionString)
    {
        services
            .AddStackExchangeRedisCache(options =>
            {
                options.Configuration = connectionString;
            });

        services.AddSingleton<IRedisDatabaseAccessor, RedisDatabaseAccessor>(
            _ => new RedisDatabaseAccessor(connectionString));

        return services;
    }

}