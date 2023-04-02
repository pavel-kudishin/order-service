using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ozon.Route256.Five.OrderService.ConsoleDbMigrator.Runner;
using Ozon.Route256.Five.OrderService.Core;
using Ozon.Route256.Five.OrderService.Core.Exceptions;
using Ozon.Route256.Five.OrderService.Db.Extensions;
using Ozon.Route256.Five.OrderService.Db.Migrations.Settings;
using Ozon.Route256.Five.OrderService.Db.Settings;

namespace Ozon.Route256.Five.OrderService.ConsoleDbMigrator;

internal static class MigrationsExtensions
{
    public static Task Migrate(this IHost host)
    {
        using IServiceScope scope = host.Services.CreateScope();
        IServiceProvider serviceProvider = scope.ServiceProvider;

        IShardMigratorRunner migratorRunner = serviceProvider.GetRequiredService<IShardMigratorRunner>();
        return migratorRunner.Migrate();
    }

    public static void ConfigureAppConfiguration(IConfigurationBuilder builder)
    {
        string environmentVariable =
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? "Development";

        builder
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile($"appsettings.{environmentVariable}.json", optional: false, reloadOnChange: false);
    }

    public static void ConfigureServiceCollection(this IServiceCollection services, IConfiguration configuration)
    {
        const string SERVICE_DISCOVERY_ADDRESS_KEY = "ROUTE256_SERVICE_DISCOVERY_ADDRESS";
        string serviceDiscoveryAddress =
            configuration[SERVICE_DISCOVERY_ADDRESS_KEY]
            ?? throw new InvalidConfigurationException(SERVICE_DISCOVERY_ADDRESS_KEY);

        services.AddGrpcClient<SdService.SdServiceClient>(
            options => { options.Address = new Uri(serviceDiscoveryAddress); });

        services.Configure<PostgresSettings>(configuration.GetSection("Postgres"));
        services.AddScoped<IShardMigratorRunner, ShardMigratorRunner>();
    }

    public static IServiceCollection ConfigureMigration(this IServiceCollection services, int bucketId, int bucketsCount)
    {
        services.AddShardingRules();

        services.Configure<MigrationSettings>(settings =>
        {
            settings.BucketId = bucketId;
            settings.BucketsCount = bucketsCount;
        });

        return services;
    }
}