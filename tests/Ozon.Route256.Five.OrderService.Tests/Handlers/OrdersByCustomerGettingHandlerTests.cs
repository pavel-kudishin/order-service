using FluentAssertions;
using Moq;
using Ozon.Route256.Five.OrderService.Core.Handlers;
using Ozon.Route256.Five.OrderService.Core.Handlers.OrdersByCustomerGet;
using Ozon.Route256.Five.OrderService.Domain.BusinessObjects;
using Ozon.Route256.Five.OrderService.Domain.Repository;

namespace Ozon.Route256.Five.OrderService.Tests.Handlers;

public class OrdersByCustomerGettingHandlerTests
{
    [Fact]
    public async Task FindByCustomer()
    {
        Mock<ICustomerRepository> customerRepository = new();
        customerRepository.Setup(repo => repo.Find(TestData.CUSTOMER_ID, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomerBo?)TestData.GetTestCustomer());

        Mock<IOrderRepository> orderRepository = new();
        DateTime startDate = DateTime.UtcNow.AddDays(-1);
        DateTime endDate = DateTime.UtcNow;
        orderRepository.Setup(repo =>
                repo.FindByCustomer(TestData.CUSTOMER_ID, startDate, endDate, 0, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestData.GetTestOrders());

        OrdersByCustomerGettingHandler handler = new(customerRepository.Object, orderRepository.Object);

        IOrdersByCustomerGettingHandler.Request request = new(TestData.CUSTOMER_ID, startDate, endDate, 0, 10);
        HandlerResult<OrderBo[]> result = await handler.Handle(request, CancellationToken.None);

        result.Success.Should().BeTrue();
        Assert.NotNull(result.Value);
        result.Value.Length.Should().Be(1);
    }
}