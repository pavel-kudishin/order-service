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
    private readonly ICustomerRepository _customerRepository;

    public OrdersGettingHandler(
        IOrderRepository orderRepository,
        IRegionRepository regionRepository,
        ICustomerRepository customerRepository)
    {
        _orderRepository = orderRepository;
        _regionRepository = regionRepository;
        _customerRepository = customerRepository;
    }

    public async Task<HandlerResult<OrderBo[]>> Handle(
        IOrdersGettingHandler.Request request,
        CancellationToken token)
    {
        RegionDto[] allRegions = await _regionRepository.GetAll(token);
        IEnumerable<string> allRegionNames = allRegions.Select(r => r.Name);

        string[]? regionNames = request.RegionNames;
        if (regionNames != null && regionNames.Length > 0)
        {
            List<string> list = regionNames.Except(allRegionNames).ToList();
            if (list.Count > 0)
            {
                return HandlerResult<OrderBo[]>.FromError(
                    new OrdersGettingException($"Regions {string.Join(',', list)} not found"));
            }
        }
        else
        {
            regionNames = allRegionNames.ToArray();
        }

        OrderDto[] orders = await _orderRepository.Filter(
            regionNames,
            request.Sources.ToOrderSourcesDto(),
            request.PageNumber,
            request.ItemsPerPage,
            request.Direction.ToOrderingDirectionDto(),
            token);

        int[] customerIds = orders.Select(o => o.CustomerId).Distinct().ToArray();
        CustomerDto[] customerDtos = await _customerRepository.FindMany(customerIds, token);

        OrderBo[] ordersBo = orders.ToOrdersBo(customerDtos);

        return HandlerResult<OrderBo[]>.FromValue(ordersBo);
    }
}