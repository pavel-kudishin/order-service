using FluentAssertions;
using Ozon.Route256.Five.OrderService.ClientBalancing;
using Ozon.Route256.Five.OrderService.Shared.ClientBalancing;

namespace Ozon.Route256.Five.OrderService.Tests;

public class DbStoreTests
{
    private readonly DbStore _dbStore;

    public DbStoreTests()
    {
        _dbStore = new DbStore();

        _dbStore.UpdateEndpointsAsync(
            new[]
            {
                new DbEndpoint("testHost1", DbReplicaType.Master),
                new DbEndpoint("testHost2", DbReplicaType.Master),
            });
    }

    [Fact]
    public async Task GetNextEndpointAsync_Successful()
    {
        DbEndpoint result = await _dbStore.GetNextEndpointAsync();
        result.ConnectionString.Should().Be("testHost1");

        result = await _dbStore.GetNextEndpointAsync();
        result.ConnectionString.Should().Be("testHost2");

        result = await _dbStore.GetNextEndpointAsync();
        result.ConnectionString.Should().Be("testHost1");
    }
}