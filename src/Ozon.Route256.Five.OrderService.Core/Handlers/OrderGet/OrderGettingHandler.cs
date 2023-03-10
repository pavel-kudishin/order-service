using Ozon.Route256.Five.OrderService.Core.BusinessObjects;
using Ozon.Route256.Five.OrderService.Core.Handlers.ResultTypes;
using Ozon.Route256.Five.OrderService.Core.Repository;
using Ozon.Route256.Five.OrderService.Core.Repository.Dto;
using Ozon.Route256.Five.OrderService.Core.Repository.Extensions;

namespace Ozon.Route256.Five.OrderService.Core.Handlers.OrderGet;

public class OrderGettingHandler : IOrderGettingHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly IRegionRepository _regionRepository;
    private readonly IAddressRepository _addressRepository;
    private readonly ICustomerRepository _customerRepository;

    public OrderGettingHandler(
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

    public async Task<HandlerResult<OrderBo>> Handle(IOrderGettingHandler.Request request, CancellationToken token)
    {
        OrderDto? order = await _orderRepository.Find(request.OrderId, token);

        if (order is null)
        {
            return HandlerResult<OrderBo>.FromError(new OrderGettingException($"Order #{request.OrderId} not found"));
        }

        AddressDto? address = await _addressRepository.Find(order.AddressId, token);

        if (address is null)
        {
            return HandlerResult<OrderBo>.FromError(
                new OrderGettingException($"Address #{order.AddressId} not found"));
        }

        RegionDto? region = await _regionRepository.Find(address.RegionId, token);

        if (region is null)
        {
            return HandlerResult<OrderBo>.FromError(
                new OrderGettingException($"Region #{address.RegionId} not found"));
        }

        CustomerDto? customer = await _customerRepository.Find(order.CustomerId, token);

        if (customer is null)
        {
            return HandlerResult<OrderBo>.FromError(
                new OrderGettingException($"Customer #{order.CustomerId} not found"));
        }

        RegionBo regionBo = region.ToRegionBo();
        CustomerBo customerBo = customer.ToCustomerBo(address: null, addresses: null);
        AddressBo? addressBo = address.ToAddressBo(regionBo);
        OrderBo orderBo = order.ToOrderBo(addressBo, customerBo);

        return HandlerResult<OrderBo>.FromValue(orderBo);
    }
}