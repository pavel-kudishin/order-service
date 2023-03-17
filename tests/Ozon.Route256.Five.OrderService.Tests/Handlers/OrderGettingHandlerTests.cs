﻿using FluentAssertions;
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

        Mock<ICustomerRepository> customerRepository = new();

        OrderGettingHandler handler = new(orderRepository.Object, customerRepository.Object);

        IOrderGettingHandler.Request request = new(1);
        HandlerResult<OrderBo> result = await handler.Handle(request, CancellationToken.None);

        result.Success.Should().BeFalse();
    }

    [Fact]
    public async Task OrderFound_Successful()
    {
        Mock<IOrderRepository> orderRepository = new();
        orderRepository.Setup(repo => repo.Find(TestData.ORDER_ID, CancellationToken.None))
            .Returns(Task.FromResult((OrderDto?)TestData.GetTestOrder()));

        Mock<ICustomerRepository> customerRepository = new();
        customerRepository.Setup(repo => repo.Find(TestData.CUSTOMER_ID, CancellationToken.None))
            .Returns(Task.FromResult((CustomerDto?)TestData.GetTestCustomer()));

        OrderGettingHandler handler =
            new(orderRepository.Object, customerRepository.Object);

        IOrderGettingHandler.Request request = new(TestData.ORDER_ID);
        HandlerResult<OrderBo> result = await handler.Handle(request, CancellationToken.None);

        result.Success.Should().BeTrue();
    }
}