using FluentAssertions;
using Moq;
using Ozon.Route256.Five.OrderService.Core.Handlers;
using Ozon.Route256.Five.OrderService.Core.Handlers.OrderGet;
using Ozon.Route256.Five.OrderService.Domain.BusinessObjects;
using Ozon.Route256.Five.OrderService.Domain.Repository;

namespace Ozon.Route256.Five.OrderService.Tests.Handlers;

public class OrderGettingHandlerTests
{
    [Fact]
    public async Task OrderNotFound()
    {
        Mock<IOrderRepository> orderRepository = new();
        orderRepository.Setup(repo => repo.Find(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrderBo?)null);

        Mock<ICustomerRepository> customerRepository = new();

        OrderGettingHandler handler = new(orderRepository.Object, customerRepository.Object);

        IOrderGettingHandler.Request request = new(1);
        HandlerResult<OrderBo> result = await handler.Handle(request, CancellationToken.None);

        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task OrderFound_Successful()
    {
        Mock<IOrderRepository> orderRepository = new();
        orderRepository.Setup(repo => repo.Find(TestData.ORDER_ID, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrderBo?)TestData.GetTestOrder());

        Mock<ICustomerRepository> customerRepository = new();
        customerRepository.Setup(repo => repo.Find(TestData.CUSTOMER_ID, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomerBo?)TestData.GetTestCustomer());

        OrderGettingHandler handler =
            new(orderRepository.Object, customerRepository.Object);

        IOrderGettingHandler.Request request = new(TestData.ORDER_ID);
        HandlerResult<OrderBo> result = await handler.Handle(request, CancellationToken.None);

        result.Success.Should().BeTrue();
    }
}