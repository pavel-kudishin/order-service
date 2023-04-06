namespace Ozon.Route256.Five.OrderService.Domain.BusinessObjects;

public sealed record OrderBo(
    long Id,
    int GoodsCount,
    decimal TotalPrice,
    decimal TotalWeight,
    OrderSourceBo Source,
    DateTimeOffset DateCreated,
    OrderStateBo State,
    CustomerBo Customer,
    AddressBo? Address,
    string Phone)
{
    public OrderStateBo State { get;  private set; } = State;

    public void Cancel()
    {
        State = OrderStateBo.Cancelled;
    }

    public void SetState(OrderStateBo newState)
    {
        State = newState;
    }
}