using Npgsql;
using Ozon.Route256.Five.OrderService.Shared.ClientBalancing;

namespace Ozon.Route256.Five.OrderService.Db.Repositories;

public class ConnectionFactory : IConnectionFactory
{
    private readonly IDbStore _dbStore;

    public ConnectionFactory(IDbStore dbStore)
    {
        _dbStore = dbStore;
    }

    public async Task<NpgsqlConnection> GetConnection()
    {
        DbEndpoint endpoint = await _dbStore.GetNextEndpointAsync();
        NpgsqlConnection connection = new(endpoint.ConnectionString);

        return connection;
    }
}