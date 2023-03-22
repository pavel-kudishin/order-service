using Ozon.Route256.Five.OrderService.Infrastructure;

namespace Ozon.Route256.Five.OrderService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await Host
                .CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>())
                .Build()
                .Migrate()
                .RunAsync();
        }
    }
}