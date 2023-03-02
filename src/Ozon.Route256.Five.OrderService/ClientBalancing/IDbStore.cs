namespace Ozon.Route256.Five.OrderService.ClientBalancing;

public interface IDbStore
{
    Task UpdateEndpointsAsync(IReadOnlyCollection<DbEndpoint> dbEndpoints);

    Task<DbEndpoint> GetNextEndpointAsync();
}