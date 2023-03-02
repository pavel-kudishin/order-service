namespace Ozon.Route256.Five.OrderService.ClientBalancing;

internal sealed class DbStore : IDbStore
{
    private DbEndpoint[] _endpoints = Array.Empty<DbEndpoint>();

    private int _currentIndex = -1;

    public Task UpdateEndpointsAsync(IReadOnlyCollection<DbEndpoint> dbEndpoints)
    {
        DbEndpoint[]? endpoints = new DbEndpoint[dbEndpoints.Count];

        int i = 0;

        foreach (DbEndpoint? endpoint in dbEndpoints)
        {
            endpoints[i++] = endpoint;
        }

        _endpoints = endpoints;

        return Task.CompletedTask;
    }

    public Task<DbEndpoint> GetNextEndpointAsync()
    {
        DbEndpoint[]? endpoints = _endpoints;

        int nextIndex = Interlocked.Increment(ref _currentIndex);

        nextIndex %= endpoints.Length;
        nextIndex = nextIndex >= 0 ? nextIndex : endpoints.Length + nextIndex;

        return Task.FromResult(endpoints[nextIndex]);
    }
}