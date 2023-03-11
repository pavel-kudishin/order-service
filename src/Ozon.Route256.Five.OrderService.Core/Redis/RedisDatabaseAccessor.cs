using System.Net;
using StackExchange.Redis;

namespace Ozon.Route256.Five.OrderService.Core.Redis;

public class RedisDatabaseAccessor : IRedisDatabaseAccessor, IDisposable
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RedisDatabaseAccessor(string? connectionString)
    {
        _connectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);
    }

    public IDatabase GetDatabase(int dbNumber = -1) => _connectionMultiplexer.GetDatabase(dbNumber);

    public IServer GetServer()
    {
        EndPoint[] endPoints = _connectionMultiplexer.GetEndPoints();
        IServer server = _connectionMultiplexer.GetServer(endPoints[0]);
        return server;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _connectionMultiplexer.Dispose();
    }

}