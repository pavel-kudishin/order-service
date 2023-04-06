using System.Data.Common;
using Npgsql;

namespace Ozon.Route256.Five.OrderService.Db.Repositories.Harness;

internal interface IConnectionFactory
{
    DbConnection GetConnectionByKey(long shardKey);

    DbConnection GetConnectionByKey(string shardKey);

    DbConnection GetConnectionByBucket(int bucketId);
}