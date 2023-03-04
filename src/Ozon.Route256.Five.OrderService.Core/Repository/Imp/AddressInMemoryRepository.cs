using Ozon.Route256.Five.OrderService.Core.Repository.Dto;

namespace Ozon.Route256.Five.OrderService.Core.Repository.Imp;

public class AddressInMemoryRepository : IAddressRepository
{
    private readonly InMemoryStorage _inMemoryStorage;

    public AddressInMemoryRepository(InMemoryStorage inMemoryStorage)
    {
        _inMemoryStorage = inMemoryStorage;
    }

    public Task<AddressDto?> Find(int addressId, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<AddressDto?>(token);
        }

        _inMemoryStorage.Addresses.TryGetValue(addressId, out AddressDto? address);

        return Task.FromResult(address).WaitAsync(token);
    }

    public Task<AddressDto[]> FindMany(IEnumerable<int> ids, CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<AddressDto[]>(token);
        }

        AddressDto[] addresses = FindDto(ids, token).ToArray();
        return Task.FromResult(addresses).WaitAsync(token);
    }

    public Task<AddressDto[]> GetAll(CancellationToken token)
    {
        if (token.IsCancellationRequested)
        {
            return Task.FromCanceled<AddressDto[]>(token);
        }

        return Task.FromResult(_inMemoryStorage.Addresses.Values.ToArray()).WaitAsync(token);

    }

    private IEnumerable<AddressDto> FindDto(IEnumerable<int> ids, CancellationToken token)
    {
        foreach (int id in ids.Distinct())
        {
            token.ThrowIfCancellationRequested();

            if (_inMemoryStorage.Addresses.TryGetValue(id, out AddressDto? address))
            {
                yield return address;
            }
        }
    }
}