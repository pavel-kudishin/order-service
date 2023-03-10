using FluentAssertions;
using Moq;
using Ozon.Route256.Five.OrderService.Core.BusinessObjects;
using Ozon.Route256.Five.OrderService.Core.Handlers.OrderGet;
using Ozon.Route256.Five.OrderService.Core.Handlers.ResultTypes;
using Ozon.Route256.Five.OrderService.Core.Repository;
using Ozon.Route256.Five.OrderService.Core.Repository.Dto;

namespace Ozon.Route256.Five.OrderService.Tests.Handlers;

public class OrderGettingHandlerTests
{
    [Fact]
    public async Task OrderNotFound()
    {
        Mock<IOrderRepository> orderRepository = new();
        orderRepository.Setup(repo => repo.Find(1, CancellationToken.None))
            .Returns(Task.FromResult<OrderDto?>(null));

        Mock<IRegionRepository> regionRepository = new();
        Mock<IAddressRepository> addressRepository = new();
        Mock<ICustomerRepository> customerRepository = new();

        OrderGettingHandler handler =
            new(orderRepository.Object, regionRepository.Object, addressRepository.Object, customerRepository.Object);

        IOrderGettingHandler.Request request = new(1);
        HandlerResult<OrderBo> result = await handler.Handle(request, CancellationToken.None);

        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task OrderFound_Successful()
    {
        Mock<IOrderRepository> orderRepository = new();
        orderRepository.Setup(repo => repo.Find(TestData.ORDER_ID, CancellationToken.None))
            .Returns(TestData.GetTestOrder());

        Mock<IRegionRepository> regionRepository = new();
        regionRepository.Setup(repo => repo.Find(TestData.REGION_ID, CancellationToken.None))
            .Returns(TestData.GetTestRegion());

        Mock<IAddressRepository> addressRepository = new();
        addressRepository.Setup(repo => repo.Find(TestData.ADDRESS_ID, CancellationToken.None))
            .Returns(TestData.GetTestAddress());

        Mock<ICustomerRepository> customerRepository = new();
        customerRepository.Setup(repo => repo.Find(TestData.CUSTOMER_ID, CancellationToken.None))
            .Returns(TestData.GetTestCustomer());

        OrderGettingHandler handler =
            new(orderRepository.Object, regionRepository.Object, addressRepository.Object, customerRepository.Object);

        IOrderGettingHandler.Request request = new(TestData.ORDER_ID);
        HandlerResult<OrderBo> result = await handler.Handle(request, CancellationToken.None);

        result.Success.Should().BeTrue();
    }
}