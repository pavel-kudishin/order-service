using FluentAssertions;
using Moq;
using Ozon.Route256.Five.OrderService.Core.BusinessObjects;
using Ozon.Route256.Five.OrderService.Core.Handlers.RegionsGet;
using Ozon.Route256.Five.OrderService.Core.Handlers.ResultTypes;
using Ozon.Route256.Five.OrderService.Core.Repository;

namespace Ozon.Route256.Five.OrderService.Tests.Handlers;

public class RegionsGettingHandlerTests
{
    [Fact]
    public async Task GetAllRegions()
    {
        Mock<IRegionRepository> regionRepository = new();
        regionRepository.Setup(repo => repo.GetAll(CancellationToken.None))
            .Returns(TestData.GetTestRegions());

        RegionsGettingHandler handler = new(regionRepository.Object);

        HandlerResult<RegionBo[]> result = await handler.Handle(CancellationToken.None);

        result.Success.Should().BeTrue();
        Assert.NotNull(result.Value);
        result.Value.Length.Should().Be(1);
    }
}