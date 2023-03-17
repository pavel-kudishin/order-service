using FluentMigrator.Runner;

namespace Ozon.Route256.Five.OrderService.Infrastructure;

public static class MigrationsExtensions
{
    public static IHost Migrate(this IHost host)
    {
        using IServiceScope scope = host.Services.CreateScope();
        IMigrationRunner runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
        runner.ListMigrations();
        runner.MigrateUp();
        return host;
    }
}