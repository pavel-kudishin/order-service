using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Ozon.Route256.Five.OrderService.Rest.Controllers;
using Ozon.Route256.Five.OrderService.Rest.Dto;
using Ozon.Route256.Five.OrderService.Rest.Validators;

namespace Ozon.Route256.Five.OrderService.Rest.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRest(this IServiceCollection services)
    {
        services.AddControllers()
            .AddApplicationPart(typeof(OrdersController).Assembly);

        services.AddScoped<IValidator<OrdersByCustomerRequestDto>, OrdersByCustomerRequestDtoValidator>();
        services.AddScoped<IValidator<AggregatedOrdersRequestDto>, AggregatedOrdersRequestDtoValidator>();

        return services;
    }
}