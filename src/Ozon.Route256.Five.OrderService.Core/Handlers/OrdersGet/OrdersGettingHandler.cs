using Ozon.Route256.Five.OrderService.Core.BusinessObjects;
using Ozon.Route256.Five.OrderService.Core.Handlers.ResultTypes;
using Ozon.Route256.Five.OrderService.Core.Repository;
using Ozon.Route256.Five.OrderService.Core.Repository.Dto;
using Ozon.Route256.Five.OrderService.Core.Repository.Extensions;

namespace Ozon.Route256.Five.OrderService.Core.Handlers.OrdersGet;

public class OrdersGettingHandler : IOrdersGettingHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly IRegionRepository _regionRepository;
    private readonly IAddressRepository _addressRepository;
    private readonly ICustomerRepository _customerRepository;

    public OrdersGettingHandler(
        IOrderRepository orderRepository,
        IRegionRepository regionRepository,
        IAddressRepository addressRepository,
        ICustomerRepository customerRepository)
    {
        _orderRepository = orderRepository;
        _regionRepository = regionRepository;
        _addressRepository = addressRepository;
        _customerRepository = customerRepository;
    }

    public async Task<HandlerResult<OrderBo[]>> Handle(IOrdersGettingHandler.Request request, CancellationToken token)
    {
        RegionDto[] regions = await _regionRepository.GetAll(token);

        if (request.RegionIds != null && request.RegionIds.Length > 0)
        {
            List<int> list = request.RegionIds.Except(regions.Select(r => r.Id)).ToList();
            if (list.Count > 0)
            {
                return HandlerResult<OrderBo[]>.FromError(
                    new OrdersGettingException($"Regions #{string.Join(',', list)} not found"));
            }
        }

        OrderDto[] orders = await _orderRepository.Filter(
            request.RegionIds,
            request.OrderTypes.ToDtoOrderTypes(),
            request.PageNumber,
            request.ItemsPerPage,
            request.Direction.ToDtoDirection(),
            token);

        OrderBo[] ordersBo = await PrepareOrdersBo(orders, regions, token);

        return HandlerResult<OrderBo[]>.FromValue(ordersBo);
    }

    private async Task<OrderBo[]> PrepareOrdersBo(OrderDto[] orders, RegionDto[] regions, CancellationToken token)
    {
        IEnumerable<int> customerIds = orders.Select(o => o.CustomerId);
        CustomerDto[] customers = await _customerRepository.FindMany(customerIds, token);

        IEnumerable<int> addressIds = orders.CollectAddressIds().Union(customers.CollectAddressIds());
        AddressDto[] addresses = await _addressRepository.FindMany(addressIds, token);

        Dictionary<int, AddressDto>? addressesDictionary = addresses.ToDictionary(a => a.Id);
        Dictionary<int, RegionDto> regionsDictionary = regions.ToDictionary(a => a.Id);
        Dictionary<int, CustomerDto> customersDictionary = customers.ToDictionary(a => a.Id);

        OrderBo[] ordersBo = orders.ToOrdersBo(addressesDictionary, regionsDictionary, customersDictionary).ToArray();
        return ordersBo;
    }
}