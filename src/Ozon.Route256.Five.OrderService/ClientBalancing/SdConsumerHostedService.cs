using Grpc.Core;
using Microsoft.Extensions.Options;
using Ozon.Route256.Five.OrderService.Shared;
using Ozon.Route256.Five.OrderService.Shared.ClientBalancing;

namespace Ozon.Route256.Five.OrderService.ClientBalancing;

public sealed class SdConsumerHostedService : BackgroundService
{
    private readonly IDbStore _dbStore;
    private readonly SdService.SdServiceClient _client;
    private readonly ILogger<SdConsumerHostedService> _logger;
    private readonly PostgresSettings _postgresSettings;

    public SdConsumerHostedService(
        IDbStore dbStore,
        SdService.SdServiceClient client,
        ILogger<SdConsumerHostedService> logger,
        IOptions<PostgresSettings> options)
    {
        _dbStore = dbStore;
        _client = client;
        _logger = logger;
        _postgresSettings = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            DbResourcesRequest request = new()
            {
                ClusterName = _postgresSettings.Cluster
            };
            using AsyncServerStreamingCall<DbResourcesResponse>? stream = _client.DbResources(
                request,
                cancellationToken: stoppingToken);

            try
            {
                while (await stream.ResponseStream.MoveNext(stoppingToken))
                {
                    DbResourcesResponse response = stream.ResponseStream.Current;
                    _logger.LogDebug(
                        "Получены новые данные из SD. Timestamp {Timestamp}",
                        response.LastUpdated.ToDateTime());

                    List<DbEndpoint> endpoints = new List<DbEndpoint>(response.Replicas.Count);

                    foreach (Replica? replica in response.Replicas)
                    {
                        DbReplicaType replicaType = GetDbReplicaType(replica.Type);
                        int[] buckets = replica.Buckets.ToArray();
                        DbEndpoint endpoint = new(replica.Host, replica.Port, replicaType, buckets);
                        endpoints.Add(endpoint);
                    }

                    _dbStore.UpdateEndpoints(endpoints);
                }
            }
            catch (RpcException exc)
            {
                _logger.LogError(exc, "Не удалось связаться с SD");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private static DbReplicaType GetDbReplicaType(Replica.Types.ReplicaType replicaType)
    {
        return replicaType switch
        {
            Replica.Types.ReplicaType.Master => DbReplicaType.Master,
            Replica.Types.ReplicaType.Sync => DbReplicaType.Sync,
            Replica.Types.ReplicaType.Async => DbReplicaType.Async,
            _ => throw new ArgumentOutOfRangeException(nameof(replicaType), replicaType, null)
        };
    }
}