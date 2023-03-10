namespace Ozon.Route256.Five.OrderService.Core.Repository.Dto;

public class AggregateOrdersDto
{
    public int RegionId { get; set; }

    public int OrdersCount { get; set; }

    public decimal TotalOrdersPrice { get; set; }

    public decimal TotalWeight { get; set; }

    public int CustomersCount { get; set; }
}