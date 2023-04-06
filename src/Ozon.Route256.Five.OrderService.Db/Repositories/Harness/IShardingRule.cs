namespace Ozon.Route256.Five.OrderService.Db.Repositories.Harness;

public interface IShardingRule<TShardKey>
{
    int GetBucketId(TShardKey key, int bucketsCount);
}