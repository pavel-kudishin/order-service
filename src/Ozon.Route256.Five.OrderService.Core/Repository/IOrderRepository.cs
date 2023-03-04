using Ozon.Route256.Five.OrderService.Core.Repository.Dto;

namespace Ozon.Route256.Five.OrderService.Core.Repository;

public interface IOrderRepository
{
    Task<OrderDto[]> GetAll(CancellationToken token);

    Task<OrderDto?> Find(long id, CancellationToken token);

    Task<OrderDto[]> FindByCustomer(int customerId, DateTime? startDate, DateTime? endDate,
        int pageNumber, int itemsPerPage, CancellationToken token);

    Task<OrderDto[]> Filter(int[]? regionIds, OrderTypesDto[]? orderTypes, int pageNumber,
        int itemsPerPage, OrderingDirectionDto orderingDirection, CancellationToken token);

    Task<AggregateOrdersDto[]> AggregateOrders(int[]? regionIds, DateTime startDate,
        DateTime? endDate, CancellationToken token);

    Task Update(OrderDto order, CancellationToken token);
}