using FluentAssertions;
using Moq;
using Ozon.Route256.Five.OrderService.Core.Abstractions;
using Ozon.Route256.Five.OrderService.Core.Handlers;
using Ozon.Route256.Five.OrderService.Core.Handlers.OrderCancel;
using Ozon.Route256.Five.OrderService.Domain.BusinessObjects;
using Ozon.Route256.Five.OrderService.Domain.Repository;

namespace Ozon.Route256.Five.OrderService.Tests.Handlers;

public class OrderCancellationHandlerTests
{
    [Fact]
    public async Task OrderCancel_Successful()
    {
        Mock<IOrderRepository> orderRepository = new();
        orderRepository.Setup(repo => repo.Find(TestData.ORDER_ID, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrderBo?)TestData.GetTestOrder());

        Mock<ILogisticService> client = new();
        client.Setup(repo => repo.OrderCancelAsync(TestData.ORDER_ID, It.IsAny<CancellationToken>()))
            .ReturnsAsync(HandlerResult.Ok);

        OrderCancellationHandler handler = new(orderRepository.Object, client.Object);

        IOrderCancellationHandler.Request request = new(TestData.ORDER_ID);
        HandlerResult result = await handler.Handle(request, CancellationToken.None);

        result.Success.Should().BeTrue();
    }
}