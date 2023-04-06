using Ozon.Route256.Five.OrderService.Domain.BusinessObjects;

namespace Ozon.Route256.Five.OrderService.Core.Handlers.OrdersByCustomerGet;

public interface IOrdersByCustomerGettingHandler: IHandler<IOrdersByCustomerGettingHandler.Request, OrderBo[]>
{
    public record Request(
        int CustomerId,
        DateTime? StartDate,
        DateTime? EndDate,
        int PageNumber,
        int ItemsPerPage);
}