using Ozon.Route256.Five.OrderService.Grpc.Extensions;
using Ozon.Route256.Five.OrderService.Host.Extensions;
using Prometheus;

namespace Ozon.Route256.Five.OrderService.Host
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddInfrastructure(_configuration);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(routeBuilder =>
            {
                routeBuilder.MapControllers();
                routeBuilder.MapGrpc();
                routeBuilder.MapMetrics();
            });
            app.UseSwagger()
                .UseSwaggerUI();
        }
    }
}
