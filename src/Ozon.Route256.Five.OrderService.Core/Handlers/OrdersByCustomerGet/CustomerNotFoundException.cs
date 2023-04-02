using Ozon.Route256.Five.OrderService.Core.Exceptions;

namespace Ozon.Route256.Five.OrderService.Core.Handlers.OrdersByCustomerGet;

public class CustomerNotFoundException : HandlerException
{
    public CustomerNotFoundException(string businessError) : base(businessError)
    {
    }
}