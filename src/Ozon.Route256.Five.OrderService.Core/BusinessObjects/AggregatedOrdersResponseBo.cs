namespace Ozon.Route256.Five.OrderService.Core.BusinessObjects;

public class AggregatedOrdersResponseBo
{
    public RegionBo? Region { get; set; }

    public int OrdersCount { get; set; }

    public decimal TotalOrdersPrice { get; set; }

    public double TotalWeight { get; set; }

    public int CustomersCount { get; set; }

}