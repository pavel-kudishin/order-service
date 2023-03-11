using Ozon.Route256.Five.OrderService.Core.Repository.Dto;

namespace Ozon.Route256.Five.OrderService.Core.Repository;

public interface IOrderRepository
{
    Task<OrderDto?> Find(long orderId, CancellationToken token);

    Task Insert(OrderDto order, CancellationToken token);

    Task Update(OrderDto order, CancellationToken token);

    Task<OrderDto[]> FindByCustomer(int customerId, DateTime? startDate, DateTime? endDate,
        int pageNumber, int itemsPerPage, CancellationToken token);

    Task<OrderDto[]> Filter(string[]? regions, OrderSourceDto[]? sources, int pageNumber,
        int itemsPerPage, OrderingDirectionDto orderingDirection, CancellationToken token);

    Task<AggregateOrdersDto[]> AggregateOrders(string[]? regionIds, DateTime startDate,
        DateTime? endDate, CancellationToken token);
}