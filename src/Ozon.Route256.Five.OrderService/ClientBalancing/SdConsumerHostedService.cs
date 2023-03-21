using Grpc.Core;
using Microsoft.Extensions.Options;
using Ozon.Route256.Five.OrderService.Core;
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
            using AsyncServerStreamingCall<DbResourcesResponse>? stream = _client.DbResources(
                new DbResourcesRequest
                {
                    ClusterName = "cluster"
                },
                cancellationToken: stoppingToken);

            try
            {
                while (await stream.ResponseStream.MoveNext(stoppingToken))
                {
                    DbResourcesResponse response = stream.ResponseStream.Current;
                    _logger.LogDebug("Получены новые данные из SD. Timestamp {Timestamp}", response.LastUpdated.ToDateTime());

                    List<DbEndpoint> endpoints = new List<DbEndpoint>(response.Replicas.Capacity);

                    foreach (Replica? replica in response.Replicas)
                    {
                        string connectionString =
                            $"Server={replica.Host};Port={replica.Port};User Id={_postgresSettings.Login};Password={_postgresSettings.Password};Database={_postgresSettings.Db};";

                        DbEndpoint endpoint = new DbEndpoint(connectionString, GetDbReplicaType(replica.Type));
                        endpoints.Add(endpoint);
                    }

                    await _dbStore.UpdateEndpointsAsync(endpoints);
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