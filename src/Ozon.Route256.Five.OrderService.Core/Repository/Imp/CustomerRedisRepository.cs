using System.Text.Json;
using System.Text.Json.Serialization;
using Ozon.Route256.Five.OrderService.Core.Redis;
using Ozon.Route256.Five.OrderService.Core.Repository.Dto;
using StackExchange.Redis;

namespace Ozon.Route256.Five.OrderService.Core.Repository.Imp;

public class CustomerRedisRepository : ICustomerRepository
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly IDatabase _redisDatabase;
    private readonly IServer _redisServer;
    private readonly ICustomerService _customerService;

    public CustomerRedisRepository(
        IRedisDatabaseAccessor redisDatabaseAccessor,
        ICustomerService customerService)
    {
        _redisDatabase = redisDatabaseAccessor.GetDatabase();
        _redisServer = redisDatabaseAccessor.GetServer();
        _customerService = customerService;
    }

    public async Task<CustomerDto?> Find(int customerId, CancellationToken token)
    {
        string key = GetKey(customerId);
        RedisValue redisValue = await _redisDatabase.StringGetAsync(key);
        if (redisValue.IsNullOrEmpty == false)
        {
            return JsonSerializer.Deserialize<CustomerDto>(redisValue.ToString(), _jsonSerializerOptions);
        }

        CustomerDto? customerDto = await _customerService.GetCustomerAsync(customerId, token);
        if (customerDto == null)
        {
            return null;
        }

        string value = JsonSerializer.Serialize(customerDto, _jsonSerializerOptions);
        await _redisDatabase.StringSetAsync(key, value);
        return customerDto;
    }

    public async Task<CustomerDto[]> GetAll(CancellationToken token)
    {
        CustomerDto[] customers = await _customerService.GetCustomersAsync(token);

        KeyValuePair<RedisKey, RedisValue>[] valuePairs = customers
            .Select(c =>
            {
                string key = GetKey(c.Id);
                string value = JsonSerializer.Serialize(c, _jsonSerializerOptions);
                return new KeyValuePair<RedisKey, RedisValue>(key, value);
            })
            .ToArray();

        await _redisDatabase.StringSetAsync(valuePairs);

        return customers;
    }

    public async Task<CustomerDto[]> FindMany(IEnumerable<int> customerIds, CancellationToken token)
    {
        IEnumerable<Task<CustomerDto?>> enumerable = customerIds.Select(id => Find(id, token));
        CustomerDto?[] customers = await Task.WhenAll(enumerable);

        return customers.Where(c => c != null).ToArray()!;
    }

    private static string GetKey(long customerId) => $"customer:{customerId}";
}