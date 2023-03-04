using Ozon.Route256.Five.OrderService.Core.Repository.Dto;

namespace Ozon.Route256.Five.OrderService.Core.Repository.Imp;

public class CustomerInMemoryRepository : ICustomerRepository
{
    private readonly InMemoryStorage _inMemoryStorage;

    public CustomerInMemoryRepository(InMemoryStorage inMemoryStorage)
    {
        _inMemoryStorage = inMemoryStorage;
    }

    public Task<CustomerDto?> Find(int customerId, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<CustomerDto?>(token);
        }

        _inMemoryStorage.Customers.TryGetValue(customerId, out CustomerDto? customer);

        return Task.FromResult(customer).WaitAsync(token);
    }

    public Task<CustomerDto[]> GetAll(CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<CustomerDto[]>(token);
        }

        return Task.FromResult(_inMemoryStorage.Customers.Values.ToArray()).WaitAsync(token);

    }

    public Task<CustomerDto[]> FindMany(IEnumerable<int> ids, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<CustomerDto[]>(token);
        }

        CustomerDto[] customers = FindDto(ids, token).ToArray();
        return Task.FromResult(customers).WaitAsync(token);
    }

    private IEnumerable<CustomerDto> FindDto(IEnumerable<int> ids, CancellationToken token)
    {
        foreach (int id in ids.Distinct())
        {
            token.ThrowIfCancellationRequested();

            if (_inMemoryStorage.Customers.TryGetValue(id, out CustomerDto? customer))
            {
                yield return customer;
            }
        }
    }
}