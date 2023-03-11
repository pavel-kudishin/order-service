using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Ozon.Route256.Five.OrderService.Core.BusinessObjects;
using Ozon.Route256.Five.OrderService.Core.Repository;
using Ozon.Route256.Five.OrderService.Core.Repository.Dto;
using Ozon.Route256.Five.OrderService.Kafka.Consumers.PreOrders;
using Ozon.Route256.Five.OrderService.Kafka.Producers.NewOrders;

namespace Ozon.Route256.Five.OrderService.Tests.Kafka.Handlers;

public class PreOrdersConsumerHandlerTests
{
    [Fact]
    public async Task Handle()
    {
        Mock<ICustomerRepository> customerRepository = new();
        customerRepository.Setup(repo => repo.Find(TestData.CUSTOMER_ID, CancellationToken.None))
            .Returns(Task.FromResult((CustomerDto?)TestData.GetTestCustomer()));

        Mock<IOrderRepository> orderRepository = new();

        Mock<ILogger<PreOrdersConsumerHandler>> logger = new();

        Mock<IRegionRepository> regionRepository = new();
        regionRepository.Setup(repo => repo.Find(TestData.REGION_NAME, CancellationToken.None))
            .Returns(Task.FromResult((RegionDto?)TestData.GetTestRegion()));

        Mock<INewOrdersKafkaPublisher> newOrdersKafkaPublisher = new();

        PreOrdersConsumerHandler handler = new(logger.Object, orderRepository.Object,
            customerRepository.Object, regionRepository.Object, newOrdersKafkaPublisher.Object);

        string key = "key";
        PreOrderAddressDto address = new(
            Region: TestData.REGION_NAME,
            City: "Moscow",
            Street: "Some Street",
            Building: "24",
            Apartment: "17",
            Latitude: 53.1,
            Longitude: 44.2
            );
        PreOrderCustomerDto customer = new(TestData.CUSTOMER_ID, address);
        PreOrderGoodsDto[] goods = new []{new PreOrderGoodsDto(100, "Name", 2, 350, 600)};
        PreOrderDto message = new(1, PreOrderSource.Api, customer, goods);
        PreOrdersConsumerHandlerResult result = await handler.Handle(key, message, CancellationToken.None);

        result.Should().Be(PreOrdersConsumerHandlerResult.Success);
    }
}