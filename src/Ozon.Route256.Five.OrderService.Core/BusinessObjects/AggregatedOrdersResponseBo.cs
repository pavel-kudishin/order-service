namespace Ozon.Route256.Five.OrderService.Core.BusinessObjects;

public class AggregatedOrdersResponseBo
{
    public RegionBo? Region { get; init; }

    public int OrdersCount { get; init; }

    public decimal TotalOrdersPrice { get; init; }

    public decimal TotalWeight { get; init; }

    public int CustomersCount { get; init; }

}