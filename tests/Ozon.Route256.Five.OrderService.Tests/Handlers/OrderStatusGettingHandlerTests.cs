using FluentAssertions;
using Moq;
using Ozon.Route256.Five.OrderService.Core.BusinessObjects;
using Ozon.Route256.Five.OrderService.Core.Handlers.OrderStatusGet;
using Ozon.Route256.Five.OrderService.Core.Handlers.ResultTypes;
using Ozon.Route256.Five.OrderService.Core.Repository;
using Ozon.Route256.Five.OrderService.Core.Repository.Dto;

namespace Ozon.Route256.Five.OrderService.Tests.Handlers;

public class OrderStatusGettingHandlerTests
{
    [Fact]
    public async Task Status_Successful()
    {
        Mock<IOrderRepository> orderRepository = new();
        orderRepository.Setup(repo => repo.Find(TestData.ORDER_ID, CancellationToken.None))
            .Returns(Task.FromResult((OrderDto?)TestData.GetTestOrder()));

        OrderStatusGettingHandler handler = new(orderRepository.Object);

        IOrderStatusGettingHandler.Request request = new(TestData.ORDER_ID);
        HandlerResult<OrderStateBo> result = await handler.Handle(request, CancellationToken.None);

        result.Success.Should().BeTrue();
    }
}