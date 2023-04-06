using System.Text;
using Murmur;

namespace Ozon.Route256.Five.OrderService.Db.Repositories.Harness;

internal sealed class RoundRobinStringShardingRule : IShardingRule<string>
{
    public int GetBucketId(string key, int bucketsCount)
    {
        int keyHashCode = GetHashCodeForKey(key);
        return Math.Abs(keyHashCode % bucketsCount);
    }

    private int GetHashCodeForKey(string key)
    {
        Murmur32? murmur = MurmurHash.Create32();
        byte[] bytes = Encoding.UTF8.GetBytes(key);
        byte[] hash = murmur.ComputeHash(bytes);
        int result = BitConverter.ToInt32(hash);
        return result;
    }
}