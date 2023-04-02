namespace Ozon.Route256.Five.OrderService.Core.Exceptions;

public class HandlerException : Exception
{
    public HandlerException(string businessError)
        : base($"Handler failed with business error: \"{businessError}\"")
    {
        BusinessError = businessError;
    }

    public string BusinessError { get; }
}