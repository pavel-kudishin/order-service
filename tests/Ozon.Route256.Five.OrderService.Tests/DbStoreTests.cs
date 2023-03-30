using FluentAssertions;
using Ozon.Route256.Five.OrderService.ClientBalancing;
using Ozon.Route256.Five.OrderService.Shared.ClientBalancing;

namespace Ozon.Route256.Five.OrderService.Tests;

public class DbStoreTests
{
    private const string HOST1 = "testHost1";
    private const string HOST2 = "testHost2";
    private const int PORT1 = 555;
    private const int PORT2 = 666;
    private const int BUCKET1 = 3;
    private const int BUCKET2 = 15;

    private readonly DbStore _dbStore;

    public DbStoreTests()
    {
        _dbStore = new DbStore();

        _dbStore.UpdateEndpoints(
            new[]
            {
                new DbEndpoint(HOST1, PORT1, DbReplicaType.Master, new []{BUCKET1}),
                new DbEndpoint(HOST2, PORT2, DbReplicaType.Master, new []{BUCKET2}),
            });
    }

    [Fact]
    public void GetNextEndpointAsync_Successful()
    {
        DbEndpoint result = _dbStore.GetEndpoint(BUCKET1);
        result.Host.Should().Be(HOST1);
        result.Port.Should().Be(PORT1);

        result = _dbStore.GetEndpoint(BUCKET2);
        result.Host.Should().Be(HOST2);
        result.Port.Should().Be(PORT2);

        result = _dbStore.GetEndpoint(BUCKET1);
        result.Host.Should().Be(HOST1);
        result.Port.Should().Be(PORT1);
    }
}