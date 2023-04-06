namespace Ozon.Route256.Five.OrderService.Core.ClientBalancing;

public interface IDbStore
{
    void UpdateEndpoints(IReadOnlyCollection<DbEndpoint> dbEndpoints);

    DbEndpoint GetEndpoint(int bucketId);

    int BucketsCount { get; }
}