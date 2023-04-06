namespace Ozon.Route256.Five.OrderService.ConsoleDbMigrator.Runner;

internal interface IShardMigratorRunner
{
    Task Migrate();
}