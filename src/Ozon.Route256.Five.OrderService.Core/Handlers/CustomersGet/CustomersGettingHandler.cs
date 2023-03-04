using Ozon.Route256.Five.OrderService.Core.BusinessObjects;
using Ozon.Route256.Five.OrderService.Core.Handlers.ResultTypes;
using Ozon.Route256.Five.OrderService.Core.Repository;
using Ozon.Route256.Five.OrderService.Core.Repository.Dto;
using Ozon.Route256.Five.OrderService.Core.Repository.Extensions;

namespace Ozon.Route256.Five.OrderService.Core.Handlers.CustomersGet;

public class CustomersGettingHandler : ICustomersGettingHandler
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IRegionRepository _regionRepository;
    private readonly IAddressRepository _addressRepository;

    public CustomersGettingHandler(
        ICustomerRepository customerRepository,
        IRegionRepository regionRepository,
        IAddressRepository addressRepository)
    {
        _customerRepository = customerRepository;
        _regionRepository = regionRepository;
        _addressRepository = addressRepository;
    }

    public async Task<HandlerResult<CustomerBo[]>> Handle(CancellationToken token)
    {
        CustomerDto[] customers = await _customerRepository.GetAll(token);
        IEnumerable<CustomerBo> customersBo = await PrepareCustomersBo(token, customers);
        return HandlerResult<CustomerBo[]>.FromValue(customersBo.ToArray());
    }

    private async Task<IEnumerable<CustomerBo>> PrepareCustomersBo(CancellationToken token, CustomerDto[] customers)
    {
        AddressDto[] addresses = await _addressRepository.GetAll(token);
        RegionDto[] regions = await _regionRepository.GetAll(token);

        Dictionary<int, AddressDto> addressesDictionary = addresses.ToDictionary(a => a.Id);
        Dictionary<int, RegionDto> regionsDictionary = regions.ToDictionary(a => a.Id);

        IEnumerable<CustomerBo> customersBo = customers.ToCustomersBo(addressesDictionary, regionsDictionary);
        return customersBo;
    }
}