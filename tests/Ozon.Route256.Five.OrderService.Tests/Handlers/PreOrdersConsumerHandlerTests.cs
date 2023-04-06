using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Ozon.Route256.Five.OrderService.Core.Handlers.PreOrders;
using Ozon.Route256.Five.OrderService.Core.Handlers.PreOrders.Dto;
using Ozon.Route256.Five.OrderService.Domain.BusinessObjects;
using Ozon.Route256.Five.OrderService.Domain.Repository;

namespace Ozon.Route256.Five.OrderService.Tests.Handlers;

public class PreOrdersConsumerHandlerTests
{
    [Fact]
    public async Task Handle()
    {
        PreOrderAddressDto address = new(
            Region: TestData.REGION_NAME,
            City: "Moscow",
            Street: "Some Street",
            Building: "24",
            Apartment: "17",
            Latitude: 53.1,
            Longitude: 44.2
        );

        Mock<ICustomerRepository> customerRepository = new();
        Expression<Func<ICustomerRepository, Task<CustomerBo?>>> findExpression =
            repo => repo.Find(TestData.CUSTOMER_ID, CancellationToken.None);
        customerRepository.Setup(findExpression).Returns(Task.FromResult((CustomerBo?)TestData.GetTestCustomer()));

        Mock<IOrderRepository> orderRepository = new();
        Mock<ILogger<PreOrdersConsumerHandler>> logger = new();

        Mock<IRegionRepository> regionRepository = new();
        Expression<Func<IRegionRepository, Task<RegionBo?>>> findRegionExpression =
            repo => repo.Find(TestData.REGION_NAME, CancellationToken.None);
        regionRepository.Setup(findRegionExpression).Returns(Task.FromResult((RegionBo?)TestData.GetTestRegion()));

        Mock<INewOrdersKafkaPublisher> newOrdersKafkaPublisher = new();
        Expression<Func<INewOrdersKafkaPublisher, Task>> publishToKafkaExpression =
            publisher => publisher.PublishToKafka(new NewOrderDto(TestData.ORDER_ID), CancellationToken.None);
        newOrdersKafkaPublisher.Setup(publishToKafkaExpression).Returns(Task.CompletedTask);

        Mock<IDistanceValidator> distanceValidator = new();
        distanceValidator.Setup(d =>
                d.IsValid(
                    address.Latitude,
                    address.Longitude,
                    TestData.GetTestWarehouse().Latitude,
                    TestData.GetTestWarehouse().Longitude))
            .Returns(true);


        PreOrdersConsumerHandler handler = new(
            logger.Object,
            orderRepository.Object,
            customerRepository.Object,
            regionRepository.Object,
            newOrdersKafkaPublisher.Object,
            distanceValidator.Object);

        string key = "key";
        PreOrderCustomerDto customer = new(TestData.CUSTOMER_ID, address);
        PreOrderGoodsDto[] goods = new[] { new PreOrderGoodsDto(100, "Name", 2, 350, 600) };
        PreOrderDto message = new(TestData.ORDER_ID, PreOrderSource.Api, customer, goods);
        PreOrdersConsumerHandlerResult result = await handler.Handle(key, message, CancellationToken.None);

        result.Should().Be(PreOrdersConsumerHandlerResult.Success);
        customerRepository.Verify(findExpression, Times.Once);
        regionRepository.Verify(findRegionExpression, Times.Once);
        newOrdersKafkaPublisher.Verify(publishToKafkaExpression, Times.Once);
    }
}