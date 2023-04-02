using Ozon.Route256.Five.OrderService.Core.Exceptions;

namespace Ozon.Route256.Five.OrderService.Core.Handlers.OrdersGet;

public class OrdersGettingException: HandlerException
{
    public OrdersGettingException(string error): base(error)
    {
    }
}