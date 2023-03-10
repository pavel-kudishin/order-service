using FluentAssertions;
using Moq;
using Ozon.Route256.Five.OrderService.Core.BusinessObjects;
using Ozon.Route256.Five.OrderService.Core.Handlers.OrdersGet;
using Ozon.Route256.Five.OrderService.Core.Handlers.ResultTypes;
using Ozon.Route256.Five.OrderService.Core.Repository;
using Ozon.Route256.Five.OrderService.Core.Repository.Dto;

namespace Ozon.Route256.Five.OrderService.Tests.Handlers;

public class OrdersGettingHandlerTests
{
    [Fact]
    public async Task GetOrders()
    {
        Mock<IRegionRepository> regionRepository = new();
        regionRepository.Setup(repo => repo.FindMany(new []{TestData.REGION_ID}, CancellationToken.None))
            .Returns(TestData.GetTestRegions());
        regionRepository.Setup(repo => repo.GetAll(CancellationToken.None))
            .Returns(TestData.GetTestRegions());

        Mock<IAddressRepository> addressRepository = new();
        addressRepository.Setup(repo => repo.FindMany(new []{TestData.ADDRESS_ID}, CancellationToken.None))
            .Returns(TestData.GetTestAddresses());

        Mock<ICustomerRepository> customerRepository = new();
        customerRepository.Setup(repo => repo.FindMany(new []{TestData.CUSTOMER_ID}, CancellationToken.None))
            .Returns(TestData.GetTestCustomers());

        Mock<IOrderRepository> orderRepository = new();
        orderRepository.Setup(repo => repo.Filter(null, null, 0, 10, OrderingDirectionDto.Asc, CancellationToken.None))
            .Returns(TestData.GetTestOrders());

        OrdersGettingHandler handler =
            new(orderRepository.Object, regionRepository.Object, addressRepository.Object, customerRepository.Object);

        IOrdersGettingHandler.Request request = new(null, null, 0, 10, OrderingDirectionBo.Asc);
        HandlerResult<OrderBo[]> result = await handler.Handle(request, CancellationToken.None);

        result.Success.Should().BeTrue();
        Assert.NotNull(result.Value);
        result.Value.Length.Should().Be(1);
    }
}