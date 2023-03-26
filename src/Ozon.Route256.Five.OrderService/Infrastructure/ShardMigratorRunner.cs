using FluentMigrator.Runner.Conventions;
using FluentMigrator.Runner;
using Grpc.Core;
using Ozon.Route256.Five.OrderService.Shared.ClientBalancing;
using Microsoft.Extensions.Options;
using Ozon.Route256.Five.OrderService.Db;
using Ozon.Route256.Five.OrderService.Shared;
using Ozon.Route256.Five.OrderService.Db.Migrations;
using Ozon.Route256.Five.OrderService.Db.Repositories;

namespace Ozon.Route256.Five.OrderService.Infrastructure;

public class ShardMigratorRunner
{
    private readonly SdService.SdServiceClient _client;
    private readonly PostgresSettings _postgresSettings;

    public ShardMigratorRunner(
        SdService.SdServiceClient client,
        IOptions<PostgresSettings> options)
    {
        _client = client;
        _postgresSettings = options.Value;
    }

    public async Task Migrate()
    {
        DbEndpoint[] endpoints = await GetEndpoints();

        int bucketsCount = endpoints.Sum(e => e.Buckets.Length);

        foreach (DbEndpoint endpoint in endpoints)
        {
            foreach (int bucketId in endpoint.Buckets)
            {
                string connectionString = CreateConnectionString(endpoint);
                IServiceProvider serviceProvider = CreateServices(connectionString, bucketId, bucketsCount);

                IMigrationRunner runner = serviceProvider.GetRequiredService<IMigrationRunner>();
                runner.MigrateUp();
            }
        }
    }

    private IServiceProvider CreateServices(string connectionString, int bucketId, int bucketsCount)
    {
        string schema = $"bucket_{bucketId}";

        ServiceProvider? provider = new ServiceCollection()
            .AddLogging(x => x.AddFluentMigratorConsole())
            .AddSingleton<IConventionSet>(new DefaultConventionSet(schema, null))
            .AddScoped<IShardingRule<string>, RoundRobinStringShardingRule>()
            .Configure<MigrationSettings>(settings =>
            {
                settings.BucketId = bucketId;
                settings.BucketsCount = bucketsCount;
            })
            .AddFluentMigratorCore()
            .ConfigureRunner(builder => builder
                .AddPostgres()
                .WithGlobalConnectionString(connectionString)
                .WithRunnerConventions(new MigrationRunnerConventions())
                .WithMigrationsIn(typeof(Migration0001).Assembly)
            )
            .BuildServiceProvider(false);
        return provider;
    }

    private string CreateConnectionString(
        DbEndpoint endpoint)
    {
        string connectionString =
            $"Server={endpoint.Host};Port={endpoint.Port};Database={_postgresSettings.Db};User Id={_postgresSettings.Login};Password={_postgresSettings.Password};";

        return connectionString;
    }

    private async Task<DbEndpoint[]> GetEndpoints()
    {
        // wait for sd
        CancellationToken token = CancellationToken.None;
        DbResourcesRequest dbResourcesRequest = new() { ClusterName = _postgresSettings.Cluster };
        using AsyncServerStreamingCall<DbResourcesResponse>? stream =
            _client.DbResources(dbResourcesRequest, cancellationToken: token);

        await stream.ResponseStream.MoveNext(CancellationToken.None);
        DbResourcesResponse? response = stream.ResponseStream.Current;
        List<DbEndpoint> endpoints = new(response.Replicas.Count);

        foreach (Replica? replica in response.Replicas)
        {
            DbEndpoint endpoint = new DbEndpoint(
                replica.Host,
                replica.Port,
                DbReplicaType.Master,
                replica.Buckets.ToArray());
            endpoints.Add(endpoint);
        }

        return endpoints.ToArray();
    }
}