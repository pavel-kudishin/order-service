using FluentAssertions;
using Moq;
using Ozon.Route256.Five.OrderService.Core.Handlers;
using Ozon.Route256.Five.OrderService.Core.Handlers.OrderStatusGet;
using Ozon.Route256.Five.OrderService.Domain.BusinessObjects;
using Ozon.Route256.Five.OrderService.Domain.Repository;

namespace Ozon.Route256.Five.OrderService.Tests.Handlers;

public class OrderStatusGettingHandlerTests
{
    [Fact]
    public async Task Status_Successful()
    {
        Mock<IOrderRepository> orderRepository = new();
        orderRepository.Setup(repo => repo.Find(TestData.ORDER_ID, CancellationToken.None))
            .Returns(Task.FromResult((OrderBo?)TestData.GetTestOrder()));

        OrderStatusGettingHandler handler = new(orderRepository.Object);

        IOrderStatusGettingHandler.Request request = new(TestData.ORDER_ID);
        HandlerResult<OrderStateBo> result = await handler.Handle(request, CancellationToken.None);

        result.Success.Should().BeTrue();
    }
}