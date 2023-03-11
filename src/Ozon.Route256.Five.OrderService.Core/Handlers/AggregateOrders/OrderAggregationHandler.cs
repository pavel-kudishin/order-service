using Ozon.Route256.Five.OrderService.Core.BusinessObjects;
using Ozon.Route256.Five.OrderService.Core.Handlers.OrdersGet;
using Ozon.Route256.Five.OrderService.Core.Handlers.ResultTypes;
using Ozon.Route256.Five.OrderService.Core.Repository;
using Ozon.Route256.Five.OrderService.Core.Repository.Dto;
using Ozon.Route256.Five.OrderService.Core.Repository.Extensions;

namespace Ozon.Route256.Five.OrderService.Core.Handlers.AggregateOrders;

public class OrderAggregationHandler: IOrderAggregationHandler
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

    public async Task<HandlerResult<AggregatedOrdersResponseBo[]>> Handle(
        IOrderAggregationHandler.Request request,
        CancellationToken token)
    {
        RegionDto[] regions = await _regionRepository.GetAll(token);

        if (request.Regions != null && request.Regions.Length > 0)
        {
            List<string> list = request.Regions.Except(regions.Select(r => r.Name)).ToList();
            if (list.Count > 0)
            {
                return HandlerResult<AggregatedOrdersResponseBo[]>.FromError(
                    new OrdersGettingException($"Regions {string.Join(',', list)} not found"));
            }
        }

        AggregateOrdersDto[] result = await _orderRepository.AggregateOrders(
            request.Regions, request.StartDate, request.EndDate, token);

        AggregatedOrdersResponseBo[] resultBo = result.ToAggregatedOrdersResponseBo();
        return HandlerResult<AggregatedOrdersResponseBo[]>.FromValue(resultBo);
    }
}