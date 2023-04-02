using Ozon.Route256.Five.OrderService.Core.Exceptions;

namespace Ozon.Route256.Five.OrderService.Core.Handlers.OrderCancel;

public class OrderCancellationException: HandlerException
{
    public OrderCancellationException(string error): base(error)
    {
    }
}