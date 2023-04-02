using Murmur;

namespace Ozon.Route256.Five.OrderService.Db.Repositories.Harness;

internal sealed class RoundRobinLongShardingRule : IShardingRule<long>
{
    public int GetBucketId(long key, int bucketsCount)
    {
        int keyHashCode = GetHashCodeForKey(key);
        return Math.Abs(keyHashCode % bucketsCount);
    }

    private static int GetHashCodeForKey(long key)
    {
        Murmur32? murmur = MurmurHash.Create32();
        byte[] bytes = BitConverter.GetBytes(key);
        byte[] hash = murmur.ComputeHash(bytes);
        int result = BitConverter.ToInt32(hash);
        return result;
    }
}