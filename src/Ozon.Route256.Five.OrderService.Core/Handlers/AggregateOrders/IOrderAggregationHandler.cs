using Ozon.Route256.Five.OrderService.Domain.BusinessObjects;

namespace Ozon.Route256.Five.OrderService.Core.Handlers.AggregateOrders;

public interface IOrderAggregationHandler
    : IHandler<IOrderAggregationHandler.Request, AggregatedOrdersBo[]>
{
    public record Request(string[]? Regions, DateTime StartDate, DateTime? EndDate);
}