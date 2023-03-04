using Ozon.Route256.Five.OrderService.Core.Repository.Dto;

namespace Ozon.Route256.Five.OrderService.Core.Repository;

public interface IAddressRepository
{
    Task<AddressDto?> Find(int addressId, CancellationToken token);

    Task<AddressDto[]> FindMany(IEnumerable<int> ids, CancellationToken token);

    Task<AddressDto[]> GetAll(CancellationToken token);

}