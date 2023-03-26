using Ozon.Route256.Five.OrderService.Infrastructure;

namespace Ozon.Route256.Five.OrderService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            IHost host = Host
                .CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>())
                .Build();

            string? environmentVariable = Environment.GetEnvironmentVariable("MIGRATE");
            bool doMigrate = environmentVariable != null && bool.TryParse(environmentVariable, out bool migrate) && migrate;

            if (doMigrate)
            {
                await host.Migrate();
            }
            else
            {
                await host.RunAsync();
            }
        }
    }
}