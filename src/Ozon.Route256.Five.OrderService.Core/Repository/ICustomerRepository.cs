using Ozon.Route256.Five.OrderService.Core.Repository.Dto;

namespace Ozon.Route256.Five.OrderService.Core.Repository;

public interface ICustomerRepository
{
    Task<CustomerDto?> Find(int customerId, CancellationToken token);

    Task<CustomerDto[]> GetAll(CancellationToken token);

    Task<CustomerDto[]> FindMany(IEnumerable<int> ids, CancellationToken token);

}