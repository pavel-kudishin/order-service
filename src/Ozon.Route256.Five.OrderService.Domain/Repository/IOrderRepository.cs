using Ozon.Route256.Five.OrderService.Domain.BusinessObjects;

namespace Ozon.Route256.Five.OrderService.Domain.Repository;

public interface IOrderRepository
{
    Task<OrderBo?> Find(long orderId, CancellationToken token);

    Task Insert(OrderBo order, CancellationToken token);

    Task Update(OrderBo order, CancellationToken token);

    Task<OrderBo[]> FindByCustomer(int customerId, DateTime? startDate, DateTime? endDate,
        int pageNumber, int itemsPerPage, CancellationToken token);

    Task<OrderBo[]> Filter(string[]? regions, OrderSourceBo[]? sources, int pageNumber,
        int itemsPerPage, OrderingDirectionBo orderingDirection, CancellationToken token);

    Task<AggregatedOrdersBo[]> AggregateOrders(string[]? regions, DateTime startDate,
        DateTime? endDate, CancellationToken token);
}