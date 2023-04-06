using Ozon.Route256.Five.OrderService.Core.Handlers.OrdersGet;
using Ozon.Route256.Five.OrderService.Domain.BusinessObjects;
using Ozon.Route256.Five.OrderService.Domain.Repository;

namespace Ozon.Route256.Five.OrderService.Core.Handlers.AggregateOrders;

internal sealed class OrderAggregationHandler: IOrderAggregationHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly IRegionRepository _regionRepository;

    public OrderAggregationHandler(
        IOrderRepository orderRepository,
        IRegionRepository regionRepository)
    {
        _orderRepository = orderRepository;
        _regionRepository = regionRepository;
    }

    public async Task<HandlerResult<AggregatedOrdersBo[]>> Handle(
        IOrderAggregationHandler.Request request,
        CancellationToken token)
    {
        RegionBo[] allRegions = await _regionRepository.GetAll(token);
        IEnumerable<string> allRegionNames = allRegions.Select(r => r.Name);

        string[]? regionNames = request.Regions;
        if (regionNames != null && regionNames.Length > 0)
        {
            List<string> list = regionNames.Except(allRegionNames).ToList();
            if (list.Count > 0)
            {
                return HandlerResult<AggregatedOrdersBo[]>.FromError(
                    new OrdersGettingException($"Regions {string.Join(',', list)} not found"));
            }
        }
        else
        {
            regionNames = allRegionNames.ToArray();
        }

        AggregatedOrdersBo[] result = await _orderRepository.AggregateOrders(
            regionNames, request.StartDate, request.EndDate, token);

        return HandlerResult<AggregatedOrdersBo[]>.FromValue(result);
    }
}