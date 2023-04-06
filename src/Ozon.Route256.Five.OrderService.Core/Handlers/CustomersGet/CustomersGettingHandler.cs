using Ozon.Route256.Five.OrderService.Domain.BusinessObjects;
using Ozon.Route256.Five.OrderService.Domain.Repository;

namespace Ozon.Route256.Five.OrderService.Core.Handlers.CustomersGet;

internal sealed class CustomersGettingHandler : ICustomersGettingHandler
{
    private readonly ICustomerRepository _customerRepository;

    public CustomersGettingHandler(
        ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<HandlerResult<CustomerBo[]>> Handle(CancellationToken token)
    {
        CustomerBo[] customers = await _customerRepository.GetAll(token);
        return HandlerResult<CustomerBo[]>.FromValue(customers);
    }
}