using Ozon.Route256.Five.OrderService.Domain.BusinessObjects;

namespace Ozon.Route256.Five.OrderService.Domain.Repository;

public interface ICustomerRepository
{
    Task<CustomerBo?> Find(int customerId, CancellationToken token);

    Task<CustomerBo[]> GetAll(CancellationToken token);

    Task<CustomerBo[]> FindMany(int[] customerIds, CancellationToken token);

}