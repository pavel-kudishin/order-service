using Ozon.Route256.Five.OrderService.Grpc.GrpcServices;
using Ozon.Route256.Five.OrderService.Infrastructure;

namespace Ozon.Route256.Five.OrderService
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
                .AddControllers()
                .Services
                .SetupGrpc(_configuration)
                .AddInfrastructure(_configuration);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(routeBuilder =>
            {
                routeBuilder.MapControllers();
                routeBuilder.MapGrpcService<OrdersService>();
                routeBuilder.MapGrpcReflectionService();
            });
            app.UseSwagger()
                .UseSwaggerUI();
        }
    }
}
