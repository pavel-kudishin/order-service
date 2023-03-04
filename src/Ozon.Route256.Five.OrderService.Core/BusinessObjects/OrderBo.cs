namespace Ozon.Route256.Five.OrderService.Core.BusinessObjects;

public class OrderBo
{
    public long Id { get; set; }
    public int ArticlesCount { get; set; }
    public decimal TotalPrice { get; set; }
    public int TotalWeight { get; set; }
    public OrderTypesBo OrderType { get; set; }
    public DateTime DateCreated { get; set; }
    public string Status { get; set; } = "";
    public CustomerBo? Customer { get; set; }
    public AddressBo? Address { get; set; }
    public string Phone { get; set; } = "";
}