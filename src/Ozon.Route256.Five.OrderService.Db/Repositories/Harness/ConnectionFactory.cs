using Microsoft.Extensions.Options;
using Npgsql;
using System.Data.Common;
using Ozon.Route256.Five.OrderService.Core;
using Ozon.Route256.Five.OrderService.Core.ClientBalancing;
using Ozon.Route256.Five.OrderService.Db.Settings;

namespace Ozon.Route256.Five.OrderService.Db.Repositories.Harness;

internal sealed class ConnectionFactory : IConnectionFactory
{
    private readonly IDbStore _dbStore;
    private readonly IShardingRule<long> _longShardingRule;
    private readonly IShardingRule<string> _stringShardingRule;
    private readonly PostgresSettings _postgresSettings;

    public ConnectionFactory(IDbStore dbStore,
        IOptions<PostgresSettings> options,
        IShardingRule<long> longShardingRule,
        IShardingRule<string> stringShardingRule)
    {
        _dbStore = dbStore;
        _longShardingRule = longShardingRule;
        _stringShardingRule = stringShardingRule;
        _postgresSettings = options.Value;
    }

    public DbConnection GetConnectionByKey(long shardKey)
    {
        int bucketId = _longShardingRule.GetBucketId(shardKey, _dbStore.BucketsCount);
        return GetConnectionByBucket(bucketId);
    }

    public DbConnection GetConnectionByKey(string shardKey)
    {
        int bucketId = _stringShardingRule.GetBucketId(shardKey, _dbStore.BucketsCount);
        return GetConnectionByBucket(bucketId);
    }

    public DbConnection GetConnectionByBucket(int bucketId)
    {
        DbEndpoint endpoint = _dbStore.GetEndpoint(bucketId);
        string connectionString =
            $"Server={endpoint.Host};Database={_postgresSettings.Db};Port={endpoint.Port};User Id={_postgresSettings.Login};Password={_postgresSettings.Password};";

        return new ShardNpgsqlConnection(new NpgsqlConnection(connectionString), bucketId);
    }
}