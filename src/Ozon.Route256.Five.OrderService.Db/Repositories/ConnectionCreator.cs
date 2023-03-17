using Npgsql;
using Ozon.Route256.Five.OrderService.Shared.ClientBalancing;

namespace Ozon.Route256.Five.OrderService.Db.Repositories;

public class ConnectionCreator : IConnectionCreator
{
    private readonly IDbStore _dbStore;

    public ConnectionCreator(IDbStore dbStore)
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