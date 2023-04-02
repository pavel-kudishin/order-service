using Ozon.Route256.Five.OrderService.Core.Exceptions;

namespace Ozon.Route256.Five.OrderService.Core.Handlers.OrderGet;

public class OrderGettingException: HandlerException
{
    public OrderGettingException(string error): base(error)
    {
    }
}