using FluentAssertions;
using Moq;
using Ozon.Route256.Five.OrderService.Core.BusinessObjects;
using Ozon.Route256.Five.OrderService.Core.Handlers.AggregateOrders;
using Ozon.Route256.Five.OrderService.Core.Handlers.ResultTypes;
using Ozon.Route256.Five.OrderService.Core.Repository;

namespace Ozon.Route256.Five.OrderService.Tests.Handlers;

public class OrderAggregationHandlerTests
{
    [Fact]
    public async Task AggregateOrders()
    {
        Mock<IRegionRepository> regionRepository = new();
        regionRepository.Setup(repo => repo.GetAll(CancellationToken.None))
            .Returns(Task.FromResult(TestData.GetTestRegions()));
        regionRepository.Setup(repo =>
                repo.FindMany(new []{ TestData .REGION_NAME}, CancellationToken.None))
            .Returns(Task.FromResult(TestData.GetTestRegions()));

        DateTime startDate = DateTime.UtcNow.AddDays(-1);
        DateTime endDate = DateTime.UtcNow;

        Mock<IOrderRepository> orderRepository = new();
        orderRepository.Setup(repo => repo.AggregateOrders(null, startDate, endDate, CancellationToken.None))
            .Returns(Task.FromResult(TestData.GetTestAggregateOrders()));

        OrderAggregationHandler handler =
            new(orderRepository.Object, regionRepository.Object);

        IOrderAggregationHandler.Request request = new(null, startDate, endDate);
        HandlerResult<AggregatedOrdersResponseBo[]> result = await handler.Handle(request, CancellationToken.None);

        result.Success.Should().BeTrue();
        Assert.NotNull(result.Value);
        result.Value.Length.Should().Be(1);
    }
}