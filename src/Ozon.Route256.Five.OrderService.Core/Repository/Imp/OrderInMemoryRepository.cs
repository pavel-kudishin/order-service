using Ozon.Route256.Five.OrderService.Core.Repository.Dto;

namespace Ozon.Route256.Five.OrderService.Core.Repository.Imp;

public class OrderInMemoryRepository : IOrderRepository
{
    private readonly InMemoryStorage _inMemoryStorage;

    public OrderInMemoryRepository(InMemoryStorage inMemoryStorage)
    {
        _inMemoryStorage = inMemoryStorage;
    }

    public Task<OrderDto[]> GetAll(CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<OrderDto[]>(token);
        }

        return Task.FromResult(_inMemoryStorage.Orders.Values.ToArray()).WaitAsync(token);
    }

    public Task<OrderDto?> Find(long id, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<OrderDto?>(token);
        }

        _inMemoryStorage.Orders.TryGetValue(id, out OrderDto? order);

        return Task.FromResult(order).WaitAsync(token);
    }

    public Task Insert(OrderDto order, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled(token);
        }

        if (_inMemoryStorage.Orders.ContainsKey(order.Id))
        {
            throw new RepositoryException($"Order#{order.Id} already exists");
        }

        _inMemoryStorage.Orders[order.Id] = order;

        return Task.CompletedTask.WaitAsync(token);
    }

    public Task<OrderDto[]> FindByCustomer(int customerId, DateTime? startDate, DateTime? endDate,
        int pageNumber, int itemsPerPage, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<OrderDto[]>(token);
        }

        IEnumerable<OrderDto> orders = _inMemoryStorage.Orders.Values
            .Where(o => o.Customer.Id == customerId);

        if (startDate.HasValue)
        {
            orders = orders.Where(o => o.DateCreated >= startDate);
        }

        if (endDate.HasValue)
        {
            orders = orders.Where(o => o.DateCreated <= endDate);
        }

        orders = orders.Skip(pageNumber * itemsPerPage).Take(itemsPerPage);

        return Task.FromResult(orders.ToArray()).WaitAsync(token);
    }

    public Task<OrderDto[]> Filter(string[]? regions, OrderSourceDto[]? sources, int pageNumber, int itemsPerPage,
        OrderingDirectionDto orderingDirection, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<OrderDto[]>(token);
        }

        IEnumerable<OrderDto> orders = _inMemoryStorage.Orders.Values;

        if (regions != null && regions.Length > 0)
        {
            orders = orders.Where(o => regions.Contains(o.Region));
        }

        if (sources != null)
        {
            orders = orders.Where(o => sources.Contains(o.Source));
        }

        if (orderingDirection == OrderingDirectionDto.Asc)
        {
            orders = orders.OrderBy(o => o.Region).ThenBy(o => o.DateCreated);
        }
        else
        {
            orders = orders.OrderByDescending(o => o.Region).ThenBy(o => o.DateCreated);
        }

        orders = orders.Skip(pageNumber * itemsPerPage).Take(itemsPerPage);

        return Task.FromResult(orders.ToArray()).WaitAsync(token);
    }

    public Task<AggregateOrdersDto[]> AggregateOrders(
        string[]? regions, DateTime startDate, DateTime? endDate, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<AggregateOrdersDto[]>(token);
        }

        IEnumerable<OrderDto> orders = _inMemoryStorage.Orders.Values
            .Where(o => o.DateCreated >= startDate);

        if (regions != null && regions.Length > 0)
        {
            orders = orders.Where(o => regions.Contains(o.Region));
        }

        if (endDate.HasValue)
        {
            orders = orders.Where(o => o.DateCreated <= endDate);
        }

        AggregateOrdersDto[] aggregateOrders = orders
            .GroupBy(o => o.Region)
            .Select(g =>
            {
                OrderDto[] orderDtos = g.ToArray();
                return new AggregateOrdersDto()
                {
                    Region = g.Key,
                    OrdersCount = orderDtos.Length,
                    TotalWeight = orderDtos.Sum(o => o.TotalWeight),
                    CustomersCount = orderDtos.Select(o => o.Customer.Id).Distinct().Count(),
                    TotalOrdersPrice = orderDtos.Sum(o => o.TotalPrice),
                };
            })
            .ToArray();


        return Task.FromResult(aggregateOrders).WaitAsync(token);
    }

    public Task Update(OrderDto order, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled(token);
        }

        if (_inMemoryStorage.Orders.ContainsKey(order.Id) == false)
        {
            throw new RepositoryException($"Order #{order.Id} not found");
        }

        _inMemoryStorage.Orders[order.Id] = order;

        return Task.CompletedTask.WaitAsync(token);
    }
}