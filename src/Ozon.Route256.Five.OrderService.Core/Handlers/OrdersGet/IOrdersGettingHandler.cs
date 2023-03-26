using Ozon.Route256.Five.OrderService.Core.BusinessObjects;

namespace Ozon.Route256.Five.OrderService.Core.Handlers.OrdersGet;

public interface IOrdersGettingHandler: IHandler<IOrdersGettingHandler.Request, OrderBo[]>
{
    public record Request(
        string[]? RegionNames,
        OrderSourceBo[]? Sources,
        int PageNumber,
        int ItemsPerPage,
        OrderingDirectionBo Direction);
}