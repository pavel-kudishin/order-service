using Ozon.Route256.Five.OrderService.Core.BusinessObjects;

namespace Ozon.Route256.Five.OrderService.Core.Handlers.AggregateOrders;

public interface IOrderAggregationHandler
    : IHandler<IOrderAggregationHandler.Request, AggregatedOrdersResponseBo[]>
{
    public record Request(int[]? RegionIds, DateTime StartDate, DateTime? EndDate);
}