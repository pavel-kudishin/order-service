using Ozon.Route256.Five.OrderService.Domain.BusinessObjects;

namespace Ozon.Route256.Five.OrderService.Core.Abstractions;

public interface ICustomerService
{
    Task<CustomerBo?> GetCustomerAsync(int customerId, CancellationToken token);
    Task<CustomerBo[]> GetCustomersAsync(CancellationToken token);
}