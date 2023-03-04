using FluentAssertions;
using Moq;
using Ozon.Route256.Five.OrderService.Core.BusinessObjects;
using Ozon.Route256.Five.OrderService.Core.Handlers.OrdersByCustomerGet;
using Ozon.Route256.Five.OrderService.Core.Handlers.ResultTypes;
using Ozon.Route256.Five.OrderService.Core.Repository;

namespace Ozon.Route256.Five.OrderService.Tests.Handlers;

public class OrdersByCustomerGettingHandlerTests
{
    [Fact]
    public async Task FindByCustomer()
    {
        Mock<IRegionRepository> regionRepository = new();
        regionRepository.Setup(repo => repo.FindMany(new []{TestData.REGION_ID}, CancellationToken.None))
            .Returns(TestData.GetTestRegions());

        Mock<IAddressRepository> addressRepository = new();
        addressRepository.Setup(repo => repo.FindMany(new []{TestData.ADDRESS_ID}, CancellationToken.None))
            .Returns(TestData.GetTestAddresses());

        Mock<ICustomerRepository> customerRepository = new();
        customerRepository.Setup(repo => repo.Find(TestData.CUSTOMER_ID, CancellationToken.None))
            .Returns(TestData.GetTestCustomer());

        Mock<IOrderRepository> orderRepository = new();
        DateTime startDate = DateTime.UtcNow.AddDays(-1);
        DateTime endDate = DateTime.UtcNow;
        orderRepository.Setup(repo => repo.FindByCustomer(TestData.CUSTOMER_ID, startDate, endDate, 0, 10, CancellationToken.None))
            .Returns(TestData.GetTestOrders());

        OrdersByCustomerGettingHandler handler =
            new(customerRepository.Object, orderRepository.Object, regionRepository.Object, addressRepository.Object);

        IOrdersByCustomerGettingHandler.Request request = new(TestData.CUSTOMER_ID, startDate, endDate, 0, 10);
        HandlerResult<OrderBo[]> result = await handler.Handle(request, CancellationToken.None);

        result.Success.Should().BeTrue();
        Assert.NotNull(result.Value);
        result.Value.Length.Should().Be(1);
    }
}