using Microsoft.Extensions.Options;
using Ozon.Route256.Five.OrderService.Shared;

namespace Ozon.Route256.Five.OrderService.Infrastructure;

public static class MigrationsExtensions
{
    public static async Task<IHost> Migrate(this IHost host)
    {
        using IServiceScope scope = host.Services.CreateScope();
        IServiceProvider serviceProvider = scope.ServiceProvider;

        SdService.SdServiceClient sdClient = serviceProvider.GetRequiredService<SdService.SdServiceClient>();
        IOptions<PostgresSettings> options = serviceProvider.GetRequiredService<IOptions<PostgresSettings>>();

        ShardMigratorRunner migratorRunner = new(sdClient, options);
        await migratorRunner.Migrate();

        return host;
    }
}