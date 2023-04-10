using FluentAssertions;
using Ozon.Route256.Five.Orders.Grpc;

namespace Ozon.Route256.Five.OrderService.IntegrationTests;

public class OrderServiceTests : IClassFixture<OrderServiceAppFactory>
{
    private readonly OrderServiceAppFactory _serviceAppFactory;

    public OrderServiceTests(OrderServiceAppFactory serviceAppFactory)
    {
        _serviceAppFactory = serviceAppFactory;
    }

    [Fact]
    public async Task GetOrderTest()
    {
        IList<OrderDto> orders = await GetOrders();

        OrderDto orderDto = orders[0];
        long orderId = orderDto.Id;

        Order order = await _serviceAppFactory.OrdersClient.GetOrderAsync(
            new GetOrderRequest()
            {
                OrderId = orderId
            });
        order.Should().NotBeNull();
        order.Id.Should().Be(orderId);
    }

    [Fact]
    public async Task GetCustomerTest()
    {
        IList<CustomerDto> customers = await _serviceAppFactory.RestClient.GetCustomersAsync(CancellationToken.None);

        customers.Should().NotBeNull();
        customers.Should().HaveCount(10);
    }

    [Fact]
    public async Task GetRegionsTest()
    {
        IList<RegionDto> regions = await _serviceAppFactory.RestClient.GetRegionsAsync(CancellationToken.None);

        regions.Should().NotBeNull();
        regions.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAggregatedOrdersTest()
    {
        AggregatedOrdersRequestDto body = new AggregatedOrdersRequestDto()
        {
            StartDate = DateTimeOffset.UtcNow.AddDays(-180),
        };
        IList<AggregatedOrdersResponseDto> response = await _serviceAppFactory.RestClient.GetAggregatedOrdersAsync(
            body,
            CancellationToken.None);

        response.Should().NotBeNull();
        response.Should().HaveCountGreaterThan(0);
    }

    [Fact]
    public async Task GetOrdersTest()
    {
        OrdersRequestDto body = new OrdersRequestDto()
        {
            PageNumber = 0,
            ItemsPerPage = 10,
        };
        IList<OrderDto> response = await _serviceAppFactory.RestClient.GetOrdersAsync(
            body,
            CancellationToken.None);

        response.Should().NotBeNull();
        response.Should().HaveCount(10);
    }

    [Fact]
    public async Task GetOrdersByCustomerTest()
    {
        IList<OrderDto> orders = await GetOrders();

        OrdersByCustomerRequestDto body = new OrdersByCustomerRequestDto()
        {
            CustomerId = orders[0].Customer.Id,
            PageNumber = 0,
            ItemsPerPage = 10,
        };
        IList<OrderDto> response = await _serviceAppFactory.RestClient.GetOrdersByCustomerAsync(
            body,
            CancellationToken.None);

        response.Should().NotBeNull();
        response.Should().HaveCount(10);
    }

    [Fact]
    public async Task GetOrderStateTest()
    {
        IList<OrderDto> orders = await GetOrders();

        OrderDto orderDto = orders[0];
        long orderId = orderDto.Id;
        OrderStatusResponseDto response = await _serviceAppFactory.RestClient.GetOrderStateAsync(
            orderId,
            CancellationToken.None);

        response.Should().NotBeNull();
        response.State.Should().Be(orderDto.State);
    }

    private async Task<IList<OrderDto>> GetOrders()
    {
        OrdersRequestDto body = new OrdersRequestDto()
        {
            PageNumber = 0,
            ItemsPerPage = 10,
        };
        IList<OrderDto> orders = await _serviceAppFactory.RestClient.GetOrdersAsync(
            body,
            CancellationToken.None);
        return orders;
    }

    [Fact]
    public async Task CancelOrderTest()
    {
        IList<OrderDto> orders = await GetOrders();

        OrderDto orderDto = orders.First(o => o.State != OrderStateDto.Cancelled);
        long orderId = orderDto.Id;
        await _serviceAppFactory.RestClient.CancelOrderAsync(
            orderId,
            CancellationToken.None);

        OrderStatusResponseDto response = await _serviceAppFactory.RestClient.GetOrderStateAsync(
            orderId,
            CancellationToken.None);

        response.Should().NotBeNull();
        response.State.Should().Be(OrderStateDto.Cancelled);
    }
}