using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Ozon.Route256.Five.OrderService.Core.Abstractions;
using Ozon.Route256.Five.OrderService.Domain.BusinessObjects;
using Ozon.Route256.Five.OrderService.Domain.Repository;
using Ozon.Route256.Five.OrderService.Redis.Dto;
using Ozon.Route256.Five.OrderService.Redis.Extensions;
using Ozon.Route256.Five.OrderService.Redis.Settings;
using StackExchange.Redis;

namespace Ozon.Route256.Five.OrderService.Redis.Repository;

internal sealed class CustomerRedisRepository : ICustomerRepository
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

    public async Task<CustomerBo?> Find(int customerId, CancellationToken token)
    {
        RedisKey key = GetKey(customerId);
        RedisValue redisValue = await _redisDatabase.StringGetAsync(key);
        if (redisValue.IsNullOrEmpty == false)
        {
            return Deserialize<CustomerDto>(redisValue).ToCustomerBo();
        }

        CustomerBo? customerBo = await _customerService.GetCustomerAsync(customerId, token);
        if (customerBo == null)
        {
            return null;
        }

        RedisValue value = Serialize(customerBo.ToCustomerDto());
        await _redisDatabase.StringSetAsync(key, value, _timeoutSettings.Customer);
        return customerBo;
    }

    public async Task<CustomerBo[]> GetAll(CancellationToken token)
    {
        RedisKey key = (RedisKey)"customers";
        RedisValue redisValue = await _redisDatabase.StringGetAsync(key);
        if (redisValue.IsNullOrEmpty == false)
        {
            return Deserialize<CustomerDto[]>(redisValue).ToCustomersBo();
        }

        CustomerBo[] customers = await _customerService.GetCustomersAsync(token);
        RedisValue value = Serialize(customers.ToCustomersDto());
        await _redisDatabase.StringSetAsync(key, value, _timeoutSettings.Customers);

        await UpdateRedis(customers);

        return customers;
    }

    public async Task<CustomerBo[]> FindMany(int[] customerIds, CancellationToken token)
    {
        RedisKey[] keys = customerIds.Select(id => GetKey(id)).ToArray();
        RedisValue[] redisValues = await _redisDatabase.StringGetAsync(keys);

        List<int> missedIds = new(keys.Length);
        List<CustomerBo> foundCustomers = new(keys.Length);

        for (int i = 0; i < redisValues.Length; i++)
        {
            if (redisValues[i].IsNull)
            {
                missedIds.Add(customerIds[i]);
            }
            else
            {
                foundCustomers.Add(Deserialize<CustomerDto>(redisValues[i]).ToCustomerBo());
            }
        }

        if (missedIds.Count > 0)
        {
            CustomerBo[] allCustomers = await _customerService.GetCustomersAsync(token);
            await UpdateRedis(allCustomers);

            IEnumerable<CustomerBo> missedCustomers = allCustomers.Where(c => missedIds.Contains(c.Id));
            foundCustomers.AddRange(missedCustomers);
        }

        return foundCustomers.ToArray();
    }

    private async Task UpdateRedis(CustomerBo[] customersBo)
    {
        CustomerDto[] customers = customersBo.ToCustomersDto();
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