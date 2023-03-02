using FluentValidation;
using Grpc.Net.ClientFactory;
using Ozon.Route256.Five.OrderService.ClientBalancing;
using Ozon.Route256.Five.OrderService.Dto;
using Ozon.Route256.Five.OrderService.GrpcServices;
using System;
using Ozon.Route256.Five.OrderService.Validators;

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
			services.AddControllers();
			services.AddGrpc(options => { options.Interceptors.Add<LoggerInterceptor>(); });
			services.AddGrpcReflection();

			services.AddSingleton<IDbStore, DbStore>();
			services.AddSingleton<LoggerInterceptor>();
			services.AddHostedService<SdConsumerHostedService>();
			services.AddGrpcClient<SdService.SdServiceClient>(
				options =>
				{
					string? address = _configuration.GetValue<string>("ROUTE256_SERVICE_DISCOVERY_ADDRESS");
					options.Address = new Uri(address);
				})
				.AddInterceptor<LoggerInterceptor>();;
			services.AddSwaggerGen();

			services.AddScoped<IValidator<OrdersByCustomerRequestDto>, OrdersByCustomerRequestDtoValidator>();
			services.AddScoped<IValidator<AggregatedOrdersRequestDto>, AggregatedOrdersRequestDtoValidator>();
			services.AddScoped<IValidator<OrdersRequestDto>, OrdersRequestDtoValidator>();
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
