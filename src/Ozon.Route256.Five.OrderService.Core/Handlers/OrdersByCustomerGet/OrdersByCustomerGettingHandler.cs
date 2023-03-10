using Ozon.Route256.Five.OrderService.Core.BusinessObjects;
using Ozon.Route256.Five.OrderService.Core.Handlers.ResultTypes;
using Ozon.Route256.Five.OrderService.Core.Repository;
using Ozon.Route256.Five.OrderService.Core.Repository.Dto;
using Ozon.Route256.Five.OrderService.Core.Repository.Extensions;

namespace Ozon.Route256.Five.OrderService.Core.Handlers.OrdersByCustomerGet;

public class OrdersByCustomerGettingHandler : IOrdersByCustomerGettingHandler
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IRegionRepository _regionRepository;
    private readonly IAddressRepository _addressRepository;

    public OrdersByCustomerGettingHandler(
        ICustomerRepository customerRepository,
        IOrderRepository orderRepository,
        IRegionRepository regionRepository,
        IAddressRepository addressRepository)
    {
        _customerRepository = customerRepository;
        _orderRepository = orderRepository;
        _regionRepository = regionRepository;
        _addressRepository = addressRepository;
    }

    public async Task<HandlerResult<OrderBo[]>> Handle(
        IOrdersByCustomerGettingHandler.Request request,
        CancellationToken token)
    {
        CustomerDto? customer = await _customerRepository.Find(request.CustomerId, token);

        if (customer is null)
        {
            return HandlerResult<OrderBo[]>.FromError(
                new CustomerNotFoundException($"Customer #{request.CustomerId} not found"));
        }

        OrderDto[] orders = await _orderRepository.FindByCustomer(
            request.CustomerId, request.StartDate, request.EndDate, request.PageNumber, request.ItemsPerPage, token);

        OrderBo[] ordersBo = await PrepareOrdersBo(token, orders);

        return HandlerResult<OrderBo[]>.FromValue(ordersBo);
    }

    private async Task<OrderBo[]> PrepareOrdersBo(CancellationToken token, OrderDto[] orders)
    {
        IEnumerable<int> addressIds = orders.Select(o => o.AddressId);

        AddressDto[] addresses = await _addressRepository.FindMany(addressIds, token);

        IEnumerable<int> regionIds = addresses.Select(a => a.RegionId);
        RegionDto[] regions = await _regionRepository.FindMany(regionIds, token);

        Dictionary<int, AddressDto> addressesDictionary = addresses.ToDictionary(a => a.Id);
        Dictionary<int, RegionDto> regionsDictionary = regions.ToDictionary(a => a.Id);

        OrderBo[] ordersBo = orders.ToOrdersBo(addressesDictionary, regionsDictionary, null).ToArray();
        return ordersBo;
    }
}