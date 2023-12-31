﻿using FluentAssertions;
using Moq;
using Ozon.Route256.Five.OrderService.Core.Handlers;
using Ozon.Route256.Five.OrderService.Core.Handlers.AggregateOrders;
using Ozon.Route256.Five.OrderService.Domain.BusinessObjects;
using Ozon.Route256.Five.OrderService.Domain.Repository;

namespace Ozon.Route256.Five.OrderService.Tests.Handlers;

public class OrderAggregationHandlerTests
{
    [Fact]
    public async Task AggregateOrders()
    {
        Mock<IRegionRepository> regionRepository = new();
        regionRepository.Setup(repo => repo.GetAll(It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestData.GetTestRegions());
        regionRepository.Setup(repo =>
                repo.FindMany(new []{ TestData .REGION_NAME}, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestData.GetTestRegions());

        DateTime startDate = DateTime.UtcNow.AddDays(-1);
        DateTime endDate = DateTime.UtcNow;

        Mock<IOrderRepository> orderRepository = new();
        orderRepository.Setup(repo =>
                repo.AggregateOrders(new[] { TestData.REGION_NAME }, startDate, endDate, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestData.GetTestAggregateOrders());

        OrderAggregationHandler handler =
            new(orderRepository.Object, regionRepository.Object);

        IOrderAggregationHandler.Request request = new(null, startDate, endDate);
        HandlerResult<AggregatedOrdersBo[]> result = await handler.Handle(request, CancellationToken.None);

        result.Success.Should().BeTrue();
        Assert.NotNull(result.Value);
        result.Value.Length.Should().Be(1);
    }
}