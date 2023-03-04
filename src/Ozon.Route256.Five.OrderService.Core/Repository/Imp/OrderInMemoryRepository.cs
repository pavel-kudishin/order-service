using Ozon.Route256.Five.OrderService.Core.Repository.Dto;

namespace Ozon.Route256.Five.OrderService.Core.Repository.Imp;

public class OrderInMemoryRepository: IOrderRepository
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

    public Task<OrderDto[]> FindByCustomer(int customerId, DateTime? startDate, DateTime? endDate,
        int pageNumber, int itemsPerPage, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<OrderDto[]>(token);
        }

        IEnumerable<OrderDto> orders = _inMemoryStorage.Orders.Values
            .Where(o => o.CustomerId == customerId);

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

    public Task<OrderDto[]> Filter(int[]? regionIds, OrderTypesDto[]? orderTypes,
        int pageNumber, int itemsPerPage,
        OrderingDirectionDto orderingDirection, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<OrderDto[]>(token);
        }

        IEnumerable<OrderDto> orders = _inMemoryStorage.Orders.Values;

        if (regionIds != null && regionIds.Length > 0)
        {
            orders = orders.Where(o => regionIds.Contains(o.RegionId));
        }

        if (orderTypes != null)
        {
            orders = orders.Where(o => orderTypes.Contains(o.OrderType));
        }

        if (orderingDirection == OrderingDirectionDto.Asc)
        {
            orders = orders.OrderBy(o => o.RegionId).ThenBy(o => o.DateCreated);
        }
        else
        {
            orders = orders.OrderByDescending(o => o.RegionId).ThenBy(o => o.DateCreated);
        }

        orders = orders.Skip(pageNumber * itemsPerPage).Take(itemsPerPage);

        return Task.FromResult(orders.ToArray()).WaitAsync(token);
    }

    public Task<AggregateOrdersDto[]> AggregateOrders(int[]? regionIds, DateTime startDate,
        DateTime? endDate, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<AggregateOrdersDto[]>(token);
        }

        IEnumerable<OrderDto> orders = _inMemoryStorage.Orders.Values
            .Where(o => o.DateCreated >= startDate);

        if (regionIds != null && regionIds.Length > 0)
        {
            orders = orders.Where(o => regionIds.Contains(o.RegionId));
        }

        if (endDate.HasValue)
        {
            orders = orders.Where(o => o.DateCreated <= endDate);
        }


        AggregateOrdersDto[] aggregateOrders = orders
            .GroupBy(o => o.RegionId)
            .Select(g => new AggregateOrdersDto()
            {
                RegionId = g.Key,
                OrdersCount = g.Count(),
                TotalWeight = g.Select(o => o.TotalWeight).Sum(),
                CustomersCount = g.Select(o => o.CustomerId).Distinct().Count(),
                TotalOrdersPrice = g.Select(o => o.TotalPrice).Sum(),
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

        _inMemoryStorage.Orders[order.Id] = order;

        return Task.CompletedTask.WaitAsync(token);
    }
}