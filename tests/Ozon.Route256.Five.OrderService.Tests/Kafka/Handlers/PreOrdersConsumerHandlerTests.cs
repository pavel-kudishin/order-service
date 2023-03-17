using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
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
        Expression<Func<ICustomerRepository, Task<CustomerDto?>>> findExpression =
            repo => repo.Find(TestData.CUSTOMER_ID, CancellationToken.None);
        customerRepository.Setup(findExpression).Returns(Task.FromResult((CustomerDto?)TestData.GetTestCustomer()));

        Mock<IOrderRepository> orderRepository = new();
        Mock<ILogger<PreOrdersConsumerHandler>> logger = new();

        Mock<IRegionRepository> regionRepository = new();
        Expression<Func<IRegionRepository, Task<RegionDto?>>> findRegionExpression =
            repo => repo.Find(TestData.REGION_NAME, CancellationToken.None);
        regionRepository.Setup(findRegionExpression).Returns(Task.FromResult((RegionDto?)TestData.GetTestRegion()));

        Mock<INewOrdersKafkaPublisher> newOrdersKafkaPublisher = new();
        Expression<Func<INewOrdersKafkaPublisher, Task>> publishToKafkaExpression =
            publisher => publisher.PublishToKafka(new NewOrderDto(TestData.ORDER_ID), CancellationToken.None);
        newOrdersKafkaPublisher.Setup(publishToKafkaExpression).Returns(Task.CompletedTask);

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
        PreOrderDto message = new(TestData.ORDER_ID, PreOrderSource.Api, customer, goods);
        PreOrdersConsumerHandlerResult result = await handler.Handle(key, message, CancellationToken.None);

        result.Should().Be(PreOrdersConsumerHandlerResult.Success);
        customerRepository.Verify(findExpression, Times.Once);
        regionRepository.Verify(findRegionExpression, Times.Once);
        newOrdersKafkaPublisher.Verify(publishToKafkaExpression, Times.Once);
    }
}