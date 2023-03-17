using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Ozon.Route256.Five.OrderService.Core.Redis;
using Ozon.Route256.Five.OrderService.Core.Redis.Settings;
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
    private readonly TimeoutSettings _timeoutSettings;

    public CustomerRedisRepository(
        IRedisDatabaseAccessor redisDatabaseAccessor,
        ICustomerService customerService,
        IOptionsSnapshot<RedisSettings> optionsSnapshot)
    {
        _redisDatabase = redisDatabaseAccessor.GetDatabase();
        _redisServer = redisDatabaseAccessor.GetServer();
        _customerService = customerService;
        _timeoutSettings = optionsSnapshot.Value.Timeouts;
    }

    public async Task<CustomerDto?> Find(int customerId, CancellationToken token)
    {
        RedisKey key = GetKey(customerId);
        RedisValue redisValue = await _redisDatabase.StringGetAsync(key);
        if (redisValue.IsNullOrEmpty == false)
        {
            return Deserialize<CustomerDto>(redisValue);
        }

        CustomerDto? customerDto = await _customerService.GetCustomerAsync(customerId, token);
        if (customerDto == null)
        {
            return null;
        }

        RedisValue value = Serialize(customerDto);
        await _redisDatabase.StringSetAsync(key, value, _timeoutSettings.Customer);
        return customerDto;
    }

    public async Task<CustomerDto[]> GetAll(CancellationToken token)
    {
        RedisKey key = (RedisKey)"customers";
        RedisValue redisValue = await _redisDatabase.StringGetAsync(key);
        if (redisValue.IsNullOrEmpty == false)
        {
            return Deserialize<CustomerDto[]>(redisValue);
        }

        CustomerDto[] customers = await _customerService.GetCustomersAsync(token);
        RedisValue value = Serialize(customers);
        await _redisDatabase.StringSetAsync(key, value, _timeoutSettings.Customers);

        await UpdateRedis(customers);

        return customers;
    }

    public async Task<CustomerDto[]> FindMany(int[] customerIds, CancellationToken token)
    {
        RedisKey[] keys = customerIds.Select(id => GetKey(id)).ToArray();
        RedisValue[] redisValues = await _redisDatabase.StringGetAsync(keys);

        List<int> missedIds = new(keys.Length);
        List<CustomerDto> foundCustomers = new(keys.Length);

        for (int i = 0; i < redisValues.Length; i++)
        {
            if (redisValues[i].IsNull)
            {
                missedIds.Add(customerIds[i]);
            }
            else
            {
                foundCustomers.Add(Deserialize<CustomerDto>(redisValues[i]));
            }
        }

        if (missedIds.Count > 0)
        {
            CustomerDto[] allCustomers = await _customerService.GetCustomersAsync(token);
            await UpdateRedis(allCustomers);

            IEnumerable<CustomerDto> missedCustomers = allCustomers.Where(c => missedIds.Contains(c.Id));
            foundCustomers.AddRange(missedCustomers);
        }

        return foundCustomers.ToArray();
    }

    private async Task UpdateRedis(CustomerDto[] customers)
    {
        for (int i = 0; i < customers.Length; i++)
        {
            CustomerDto customer = customers[i];
            RedisKey key = GetKey(customer.Id);
            RedisValue value = Serialize(customer);
            await _redisDatabase.StringSetAsync(key, value, _timeoutSettings.Customer);
        }
    }

    private static T Deserialize<T>(RedisValue v)
    {
        return JsonSerializer.Deserialize<T>(v.ToString(), _jsonSerializerOptions)!;
    }

    private static RedisValue Serialize<T>(T obj)
    {
        return JsonSerializer.Serialize(obj, _jsonSerializerOptions);
    }

    private static RedisKey GetKey(long customerId) => (RedisKey)$"customer:{customerId}";
}