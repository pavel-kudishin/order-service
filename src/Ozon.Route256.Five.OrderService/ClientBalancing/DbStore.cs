using Ozon.Route256.Five.OrderService.Shared.ClientBalancing;

namespace Ozon.Route256.Five.OrderService.ClientBalancing;

internal sealed class DbStore : IDbStore
{
    private Dictionary<int, DbEndpoint> _bucketsDictionary = new(0);

    public int BucketsCount => _bucketsDictionary.Count;

    public void UpdateEndpoints(IReadOnlyCollection<DbEndpoint> dbEndpoints)
    {
        Dictionary<int, DbEndpoint> bucketsDictionary = new();

        foreach (DbEndpoint endpoint in dbEndpoints)
        {
            foreach (int bucket in endpoint.Buckets)
            {
                bucketsDictionary.Add(bucket, endpoint);
            }
        }

        _bucketsDictionary = bucketsDictionary;
    }

    public DbEndpoint GetEndpoint(int bucketId)
    {
        Dictionary<int, DbEndpoint> bucketsDictionary = _bucketsDictionary;

        if (bucketsDictionary.TryGetValue(bucketId, out DbEndpoint? endpoint))
        {
            return endpoint;
        }

        throw new ArgumentOutOfRangeException($"There is no endpoint for bucket {bucketId}");
    }
}