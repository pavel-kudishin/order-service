using FluentAssertions;
using Ozon.Route256.Five.OrderService.ClientBalancing;
using Ozon.Route256.Five.OrderService.Shared.ClientBalancing;

namespace Ozon.Route256.Five.OrderService.Tests;

public class DbStoreTests
{
    private const string HOST1 = "testHost1";
    private const string HOST2 = "testHost2";
    private const int PORT1 = 555;
    protected const int PORT2 = 666;

    private readonly DbStore _dbStore;

    public DbStoreTests()
    {
        _dbStore = new DbStore();

        _dbStore.UpdateEndpointsAsync(
            new[]
            {
                new DbEndpoint(HOST1, PORT1, DbReplicaType.Master),
                new DbEndpoint(HOST2, PORT2, DbReplicaType.Master),
            });
    }

    [Fact]
    public async Task GetNextEndpointAsync_Successful()
    {
        DbEndpoint result = await _dbStore.GetNextEndpointAsync();
        result.Host.Should().Be(HOST1);
        result.Port.Should().Be(PORT1);

        result = await _dbStore.GetNextEndpointAsync();
        result.Host.Should().Be(HOST2);
        result.Port.Should().Be(PORT2);

        result = await _dbStore.GetNextEndpointAsync();
        result.Host.Should().Be(HOST1);
        result.Port.Should().Be(PORT1);
    }
}