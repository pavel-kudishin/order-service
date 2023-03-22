namespace Ozon.Route256.Five.OrderService.Shared.ClientBalancing;

public interface IDbStore
{
    Task UpdateEndpointsAsync(IReadOnlyCollection<DbEndpoint> dbEndpoints);

    Task<DbEndpoint> GetNextEndpointAsync();
}