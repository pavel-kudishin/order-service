using Microsoft.Extensions.Hosting;

namespace Ozon.Route256.Five.OrderService.ConsoleDbMigrator
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await Microsoft.Extensions.Hosting.Host
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