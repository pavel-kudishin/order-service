using FluentAssertions;
using Moq;
using Ozon.Route256.Five.OrderService.Core;
using Ozon.Route256.Five.OrderService.Core.Handlers.OrderCancel;
using Ozon.Route256.Five.OrderService.Core.Handlers.ResultTypes;
using Ozon.Route256.Five.OrderService.Core.Repository;
using Ozon.Route256.Five.OrderService.Core.Repository.Dto;

namespace Ozon.Route256.Five.OrderService.Tests.Handlers;

public class OrderCancellationHandlerTests
{
    [Fact]
    public async Task OrderCancel_Successful()
    {
        Mock<IOrderRepository> orderRepository = new();
        orderRepository.Setup(repo => repo.Find(TestData.ORDER_ID, CancellationToken.None))
            .Returns(Task.FromResult((OrderDto?)TestData.GetTestOrder()));

        Mock<ILogisticService> client = new();
        client.Setup(repo => repo.OrderCancelAsync(TestData.ORDER_ID, CancellationToken.None))
            .Returns(Task.FromResult<HandlerResult>(HandlerResult.Ok));

        OrderCancellationHandler handler = new(orderRepository.Object, client.Object);

        IOrderCancellationHandler.Request request = new(TestData.ORDER_ID);
        HandlerResult result = await handler.Handle(request, CancellationToken.None);

        result.Success.Should().BeTrue();
    }
}