namespace Ozon.Route256.Five.OrderService.Core.Exceptions;

public sealed class OrderNotFoundException : HandlerException
{
    public OrderNotFoundException(string businessError) : base(businessError)
    {
    }
}