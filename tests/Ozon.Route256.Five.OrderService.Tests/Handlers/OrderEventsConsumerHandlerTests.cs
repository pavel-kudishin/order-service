using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Ozon.Route256.Five.OrderService.Core.Handlers.OrderEvents;
using Ozon.Route256.Five.OrderService.Core.Handlers.OrderEvents.Dto;
using Ozon.Route256.Five.OrderService.Domain.BusinessObjects;
using Ozon.Route256.Five.OrderService.Domain.Repository;

namespace Ozon.Route256.Five.OrderService.Tests.Handlers;

public class OrderEventsConsumerHandlerTests
{
    [Fact]
    public async Task Handle()
    {
        OrderBo orderDto = TestData.GetTestOrder();

        Mock<IOrderRepository> orderRepository = new();
        Expression<Func<IOrderRepository, Task<OrderBo?>>> findExpression =
            repo => repo.Find(TestData.ORDER_ID, CancellationToken.None);
        orderRepository.Setup(findExpression).Returns(Task.FromResult((OrderBo?)orderDto));

        orderDto.SetState(OrderStateBo.Delivered);

        Expression<Func<IOrderRepository, Task>> updateExpression =
            repo => repo.Update(orderDto, CancellationToken.None);
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