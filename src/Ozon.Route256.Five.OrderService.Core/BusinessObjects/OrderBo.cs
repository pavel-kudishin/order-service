namespace Ozon.Route256.Five.OrderService.Core.BusinessObjects;

public class OrderBo
{
    public long Id { get; init; }
    public int ArticlesCount { get; init; }
    public decimal TotalPrice { get; init; }
    public decimal TotalWeight { get; init; }
    public OrderTypesBo OrderType { get; init; }
    public DateTime DateCreated { get; init; }
    public string Status { get; init; } = string.Empty;
    public CustomerBo? Customer { get; init; }
    public AddressBo? Address { get; init; }
    public string Phone { get; init; } = string.Empty;
}