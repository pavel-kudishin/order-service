using System.Text.Json;
using System.Text.Json.Serialization;
using Ozon.Route256.Five.OrderService.Core.BusinessObjects;
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
        RedisKey key = GetKey(customerId);
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
        RedisKey key = (RedisKey)"customers";
        RedisValue redisValue = await _redisDatabase.StringGetAsync(key);
        if (redisValue.IsNullOrEmpty == false)
        {
            return JsonSerializer.Deserialize<CustomerDto[]>(redisValue.ToString(), _jsonSerializerOptions)!;
        }

        CustomerDto[] customers = await _customerService.GetCustomersAsync(token);
        string value = JsonSerializer.Serialize(customers, _jsonSerializerOptions);
        await _redisDatabase.StringSetAsync(key, value);

        await UpdateRedis(customers);

        return customers;
    }

    public async Task<CustomerDto[]> FindMany(int[] customerIds, CancellationToken token)
    {
        RedisKey[] keys = customerIds.Select(id => GetKey(id)).ToArray();
        RedisValue[] redisValues = await _redisDatabase.StringGetAsync(keys);

        List<int> missedIds = new(keys.Length);

        for (int i = 0; i < redisValues.Length; i++)
        {
            if (redisValues[i].IsNull)
            {
                missedIds.Add(customerIds[i]);
            }
        }

        IEnumerable<CustomerDto> customers = redisValues
            .Where(v => v.IsNullOrEmpty == false)
            .Select(v => JsonSerializer.Deserialize<CustomerDto>(v.ToString(), _jsonSerializerOptions)!);

        if (missedIds.Count > 0)
        {
            CustomerDto[] allCustomers = await _customerService.GetCustomersAsync(token);
            await UpdateRedis(allCustomers);

            IEnumerable<CustomerDto> missedCustomers = allCustomers.Where(c => missedIds.Contains(c.Id));
            customers = customers.Union(missedCustomers);
        }

        return customers.ToArray();
    }

    private async Task UpdateRedis(CustomerDto[] customers)
    {
        KeyValuePair<RedisKey, RedisValue>[] valuePairs = customers
            .Select(c =>
            {
                RedisKey key = GetKey(c.Id);
                string value = JsonSerializer.Serialize(c, _jsonSerializerOptions);
                return new KeyValuePair<RedisKey, RedisValue>(key, value);
            })
            .ToArray();

        await _redisDatabase.StringSetAsync(valuePairs);
    }


    private static RedisKey GetKey(long customerId) => (RedisKey)$"customer:{customerId}";
}