using Microsoft.Extensions.Hosting;

namespace Ozon.Route256.Five.OrderService.ConsoleDbMigrator
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await Host
                .CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(MigrationsExtensions.ConfigureAppConfiguration)
                .ConfigureServices((context, services) =>
                {
                    services.ConfigureServiceCollection(context.Configuration);
                })
                .Build()
                .Migrate();
        }
    }
}