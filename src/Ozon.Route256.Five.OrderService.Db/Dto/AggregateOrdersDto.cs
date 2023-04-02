namespace Ozon.Route256.Five.OrderService.Db.Dto;

internal class AggregateOrdersDto
{
    public string Region { get; init; } = string.Empty;

    public int OrdersCount { get; init; }

    public decimal TotalOrdersPrice { get; init; }

    public decimal TotalWeight { get; init; }

    public int CustomersCount { get; init; }
}