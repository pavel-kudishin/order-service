using Ozon.Route256.Five.OrderService.Core.BusinessObjects;
using Ozon.Route256.Five.OrderService.Core.Handlers.ResultTypes;
using Ozon.Route256.Five.OrderService.Core.Repository;
using Ozon.Route256.Five.OrderService.Core.Repository.Dto;
using Ozon.Route256.Five.OrderService.Core.Repository.Extensions;

namespace Ozon.Route256.Five.OrderService.Core.Handlers.CustomersGet;

public class CustomersGettingHandler : ICustomersGettingHandler
{
    private readonly ICustomerRepository _customerRepository;

    public CustomersGettingHandler(
        ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<HandlerResult<CustomerBo[]>> Handle(CancellationToken token)
    {
        CustomerDto[] customers = await _customerRepository.GetAll(token);
        CustomerBo[] customersBo = customers.ToCustomersBo();
        return HandlerResult<CustomerBo[]>.FromValue(customersBo);
    }
}