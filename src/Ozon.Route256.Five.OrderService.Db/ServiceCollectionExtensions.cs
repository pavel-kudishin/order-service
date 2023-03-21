using Dapper;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ozon.Route256.Five.OrderService.Core;
using Ozon.Route256.Five.OrderService.Db.Migrations;
using Ozon.Route256.Five.OrderService.Db.Repositories;

namespace Ozon.Route256.Five.OrderService.Db;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMigrations(this IServiceCollection services, IConfiguration configuration)
    {
        SqlMapper.AddTypeHandler(new WarehouseDtoTypeMapper());

        services.AddScoped<IConnectionFactory, ConnectionFactory>();

        const string PG_CONNECTION_STRING_KEY = "ROUTE256_PG_CONNECTION_STRING";
        string pgConnectionString =
            configuration.GetValue<string>(PG_CONNECTION_STRING_KEY)
            ?? throw new InvalidConfigurationException(PG_CONNECTION_STRING_KEY);

        services
            .AddLogging(x => x.AddFluentMigratorConsole())
            .AddFluentMigratorCore()
            .ConfigureRunner(builder => builder
                .AddPostgres()
                .WithGlobalConnectionString(pgConnectionString)
                .WithMigrationsIn(typeof(Migration0001).Assembly));

        return services;
    }
}