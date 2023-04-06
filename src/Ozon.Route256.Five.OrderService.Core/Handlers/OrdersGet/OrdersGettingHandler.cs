using Ozon.Route256.Five.OrderService.Domain.BusinessObjects;
using Ozon.Route256.Five.OrderService.Domain.Repository;

namespace Ozon.Route256.Five.OrderService.Core.Handlers.OrdersGet;

internal sealed class OrdersGettingHandler : IOrdersGettingHandler
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
        RegionBo[] allRegions = await _regionRepository.GetAll(token);
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

        OrderBo[] orders = await _orderRepository.Filter(
            regionNames,
            request.Sources,
            request.PageNumber,
            request.ItemsPerPage,
            request.Direction,
            token);

        return HandlerResult<OrderBo[]>.FromValue(orders);
    }
}