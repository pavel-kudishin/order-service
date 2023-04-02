using System.Net;
using Microsoft.Extensions.Options;
using Ozon.Route256.Five.OrderService.Redis.Settings;
using StackExchange.Redis;

namespace Ozon.Route256.Five.OrderService.Redis;

internal sealed class RedisDatabaseAccessor : IRedisDatabaseAccessor, IDisposable
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RedisDatabaseAccessor(IOptions<RedisSettings> options)
    {
        _connectionMultiplexer = ConnectionMultiplexer.Connect(options.Value.ConnectionString);
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