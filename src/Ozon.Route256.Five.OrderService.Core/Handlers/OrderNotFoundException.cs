namespace Ozon.Route256.Five.OrderService.Core.Handlers;

public class OrderNotFoundException : HandlerException
{
    public OrderNotFoundException(string businessError) : base(businessError)
    {
    }
}