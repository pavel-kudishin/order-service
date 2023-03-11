using Ozon.Route256.Five.OrderService.Core.Repository.Dto;

namespace Ozon.Route256.Five.OrderService.Core;

public interface ICustomerService
{
    Task<CustomerDto?> GetCustomerAsync(int customerId, CancellationToken token);
    Task<CustomerDto[]> GetCustomersAsync(CancellationToken token);
}