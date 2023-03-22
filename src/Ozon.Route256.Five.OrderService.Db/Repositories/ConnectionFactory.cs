using Microsoft.Extensions.Options;
using Npgsql;
using Ozon.Route256.Five.OrderService.Shared;
using Ozon.Route256.Five.OrderService.Shared.ClientBalancing;

namespace Ozon.Route256.Five.OrderService.Db.Repositories;

public class ConnectionFactory : IConnectionFactory
{
    private readonly IDbStore _dbStore;
    private readonly PostgresSettings _postgresSettings;

    public ConnectionFactory(IDbStore dbStore,
        IOptions<PostgresSettings> options)
    {
        _dbStore = dbStore;
        _postgresSettings = options.Value;
    }

    public async Task<NpgsqlConnection> GetConnection()
    {
        DbEndpoint endpoint = await _dbStore.GetNextEndpointAsync();
        string connectionString =
            $"Server={endpoint.Host};Port={endpoint.Port};User Id={_postgresSettings.Login};Password={_postgresSettings.Password};Database={_postgresSettings.Db};";

        NpgsqlConnection connection = new(connectionString);

        return connection;
    }
}