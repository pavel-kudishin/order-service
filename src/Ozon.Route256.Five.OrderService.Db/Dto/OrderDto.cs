namespace Ozon.Route256.Five.OrderService.Db.Dto;

internal class OrderDto
{
    public long Id { get; init; }
    public int GoodsCount { get; init; }
    public decimal TotalPrice { get; init; }
    public decimal TotalWeight { get; init; }
    public OrderSourceDto Source { get; init; }
    public DateTime DateCreated { get; init; }
    public OrderStateDto State { get; set; }
    public int CustomerId { get; init; }
    public AddressDto? Address { get; set; }
    public string Phone { get; init; } = string.Empty;
}