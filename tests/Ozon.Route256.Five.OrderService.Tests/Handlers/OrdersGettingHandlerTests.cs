using FluentAssertions;
using Moq;
using Ozon.Route256.Five.OrderService.Core.Handlers;
using Ozon.Route256.Five.OrderService.Core.Handlers.OrdersGet;
using Ozon.Route256.Five.OrderService.Domain.BusinessObjects;
using Ozon.Route256.Five.OrderService.Domain.Repository;

namespace Ozon.Route256.Five.OrderService.Tests.Handlers;

public class OrdersGettingHandlerTests
{
    [Fact]
    public async Task GetOrders()
    {
        Mock<IRegionRepository> regionRepository = new();
        regionRepository.Setup(repo =>
                repo.FindMany(new []{TestData.REGION_NAME}, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestData.GetTestRegions());
        regionRepository.Setup(repo => repo.GetAll(It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestData.GetTestRegions());

        Mock<IOrderRepository> orderRepository = new();
        orderRepository.Setup(repo =>
                repo.Filter(new[] { TestData.REGION_NAME }, null, 0, 10,
                    OrderingDirectionBo.Asc, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestData.GetTestOrders());

        Mock<ICustomerRepository> customerRepository = new();
        customerRepository.Setup(repo => repo.Find(TestData.CUSTOMER_ID, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CustomerBo?)TestData.GetTestCustomer());

        OrdersGettingHandler handler = new(orderRepository.Object, regionRepository.Object, customerRepository.Object);

        IOrdersGettingHandler.Request request = new(null, null, 0, 10, OrderingDirectionBo.Asc);
        HandlerResult<OrderBo[]> result = await handler.Handle(request, CancellationToken.None);

        result.Success.Should().BeTrue();
        Assert.NotNull(result.Value);
        result.Value.Length.Should().Be(1);
    }
}