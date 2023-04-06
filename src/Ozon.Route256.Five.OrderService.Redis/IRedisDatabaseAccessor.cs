using StackExchange.Redis;

namespace Ozon.Route256.Five.OrderService.Redis;

internal interface IRedisDatabaseAccessor
{
    IDatabase GetDatabase(int dbNumber = -1);

    IServer GetServer();
}