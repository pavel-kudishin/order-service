using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Ozon.Route256.Five.OrderService.Core.Repository;
using Ozon.Route256.Five.OrderService.Core.Repository.Dto;
using Ozon.Route256.Five.OrderService.Kafka.Consumers.OrderEvents;
using OrderStateDto = Ozon.Route256.Five.OrderService.Kafka.Consumers.OrderEvents.OrderStateDto;

namespace Ozon.Route256.Five.OrderService.Tests.Kafka.Handlers;

public class OrderEventsConsumerHandlerTests
{
    [Fact]
    public async Task Handle()
    {
        Mock<IOrderRepository> orderRepository = new();
        orderRepository.Setup(repo => repo.Find(TestData.ORDER_ID, CancellationToken.None))
            .Returns(Task.FromResult((OrderDto?)TestData.GetTestOrder()));

        Mock<ILogger<OrderEventsConsumerHandler>> logger = new();

        OrderEventsConsumerHandler handler = new(logger.Object, orderRepository.Object);

        string key = "key";
        OrderEventsChangedDto message = new(TestData.ORDER_ID, OrderStateDto.Delivered, DateTimeOffset.UtcNow);
        OrderEventsConsumerHandlerResult result = await handler.Handle(key, message, CancellationToken.None);

        result.Should().Be(OrderEventsConsumerHandlerResult.Success);
    }
}