using System.Linq.Expressions;
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
        OrderDto orderDto = TestData.GetTestOrder();

        Mock<IOrderRepository> orderRepository = new();
        Expression<Func<IOrderRepository, Task<OrderDto?>>> findExpression =
            repo => repo.Find(TestData.ORDER_ID, CancellationToken.None);
        orderRepository.Setup(findExpression).Returns(Task.FromResult((OrderDto?)orderDto));

        OrderDto updatedOrder = orderDto with
        {
            State = Core.Repository.Dto.OrderStateDto.Delivered,
        };

        Expression<Func<IOrderRepository, Task>> updateExpression =
            repo => repo.Update(updatedOrder, CancellationToken.None);
        orderRepository.Setup(updateExpression).Returns(Task.CompletedTask);

        Mock<ILogger<OrderEventsConsumerHandler>> logger = new();

        OrderEventsConsumerHandler handler = new(logger.Object, orderRepository.Object);

        OrderEventsChangedDto message = new(TestData.ORDER_ID, OrderStateDto.Delivered, DateTimeOffset.UtcNow);
        OrderEventsConsumerHandlerResult result = await handler.Handle("key", message, CancellationToken.None);

        result.Should().Be(OrderEventsConsumerHandlerResult.Success);
        orderRepository.Verify(findExpression, Times.Once);
        orderRepository.Verify(updateExpression, Times.Once);
    }
}