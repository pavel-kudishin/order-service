using Serilog;

namespace Ozon.Route256.Five.OrderService.Host
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await Microsoft.Extensions.Hosting.Host
                .CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>())
                .UseSerilog((builderContext, configuration) => configuration.WriteTo.Console())
                .Build()
                .RunAsync();
        }
    }
}