using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Ozon.Route256.Five.OrderService.Core.Redis;
using Ozon.Route256.Five.OrderService.Core.Repository.Dto;
using StackExchange.Redis;

namespace Ozon.Route256.Five.OrderService.Core.Repository.Imp;

public class OrderRedisRepository : IOrderRepository
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly IDatabase _redisDatabase;
    private readonly IServer _redisServer;

    public OrderRedisRepository(IRedisDatabaseAccessor redisDatabaseAccessor)
    {
        _redisDatabase = redisDatabaseAccessor.GetDatabase();
        _redisServer = redisDatabaseAccessor.GetServer();
    }

    private static string GetKey(long orderId) => $"order:{orderId}";

    public async Task<OrderDto?> Find(long orderId, CancellationToken token)
    {
        RedisValue redisValue = await _redisDatabase.StringGetAsync(GetKey(orderId));
        if (redisValue.IsNullOrEmpty)
        {
            return null;
        }

        return JsonSerializer.Deserialize<OrderDto>(redisValue, _jsonSerializerOptions);
    }

    public async Task Insert(OrderDto order, CancellationToken token)
    {
        string key = GetKey(order.Id);
        bool isKeyFound = await _redisDatabase.KeyExistsAsync(key);
        if (isKeyFound)
        {
            throw new RepositoryException($"Order #{order.Id} already exists");
        }

        await _redisDatabase.StringSetAsync(key, JsonSerializer.Serialize(order, _jsonSerializerOptions));
    }

    public async Task Update(OrderDto order, CancellationToken token)
    {
        string key = GetKey(order.Id);
        bool isKeyFound = await _redisDatabase.KeyExistsAsync(key);
        if (isKeyFound == false)
        {
            throw new RepositoryException($"Order #{order.Id} not found");
        }

        string newValue = JsonSerializer.Serialize(order, _jsonSerializerOptions);
        await _redisDatabase.StringSetAsync(key, newValue);
    }

    public async Task<OrderDto[]> GetAll(CancellationToken token)
    {
        IAsyncEnumerable<RedisKey> keys = _redisServer.KeysAsync(pattern: "order:*");
        List<Task<RedisValue>> tasks = await GetValues(keys, token).ToListAsync(token);
        RedisValue[] result = await Task.WhenAll(tasks);
        OrderDto[] array = result
            .Select(r => JsonSerializer.Deserialize<OrderDto>(r, _jsonSerializerOptions)!)
            .ToArray();
        return array;
    }

    private async IAsyncEnumerable<Task<RedisValue>> GetValues(
        IAsyncEnumerable<RedisKey> keys,
        [EnumeratorCancellation] CancellationToken token)
    {
        await foreach (RedisKey key in keys.WithCancellation(token))
        {
            yield return _redisDatabase.StringGetAsync(key);
        }
    }

    public async Task<OrderDto[]> FindByCustomer(int customerId, DateTime? startDate, DateTime? endDate,
        int pageNumber, int itemsPerPage, CancellationToken token)
    {
        OrderDto[] allOrders = await GetAll(token);

        IEnumerable<OrderDto> orders = allOrders.Where(o => o.Customer.Id == customerId);

        if (startDate.HasValue)
        {
            orders = orders.Where(o => o.DateCreated >= startDate);
        }

        if (endDate.HasValue)
        {
            orders = orders.Where(o => o.DateCreated <= endDate);
        }

        orders = orders.Skip(pageNumber * itemsPerPage).Take(itemsPerPage);

        return orders.ToArray();
    }

    public async Task<OrderDto[]> Filter(string[]? regions, OrderSourceDto[]? sources,
        int pageNumber, int itemsPerPage,
        OrderingDirectionDto orderingDirection, CancellationToken token)
    {
        IEnumerable<OrderDto> orders = await GetAll(token);

        if (regions != null && regions.Length > 0)
        {
            orders = orders.Where(o => regions.Contains(o.Region));
        }

        if (sources != null && sources.Length > 0)
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

        return orders.ToArray();
    }

    public async Task<AggregateOrdersDto[]> AggregateOrders(string[]? regionIds, DateTime startDate,
        DateTime? endDate, CancellationToken token)
    {
        OrderDto[] allOrders = await GetAll(token);
        IEnumerable<OrderDto> orders = allOrders.Where(o => o.DateCreated >= startDate);

        if (regionIds != null && regionIds.Length > 0)
        {
            orders = orders.Where(o => regionIds.Contains(o.Region));
        }

        if (endDate.HasValue)
        {
            orders = orders.Where(o => o.DateCreated <= endDate);
        }

        AggregateOrdersDto[] aggregateOrders = orders
            .GroupBy(o => o.Region)
            .Select(g => new AggregateOrdersDto()
            {
                Region = g.Key,
                OrdersCount = g.Count(),
                TotalWeight = g.Sum(o => o.TotalWeight),
                CustomersCount = g.Select(o => o.Customer.Id).Distinct().Count(),
                TotalOrdersPrice = g.Sum(o => o.TotalPrice),
            })
            .ToArray();

        return aggregateOrders;
    }
}