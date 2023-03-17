using StackExchange.Redis;

namespace Ozon.Route256.Five.OrderService.Core.Redis;

public interface IRedisDatabaseAccessor
{
    IDatabase GetDatabase(int dbNumber = -1);

    IServer GetServer();
}