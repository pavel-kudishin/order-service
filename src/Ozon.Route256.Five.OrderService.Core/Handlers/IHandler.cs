using Ozon.Route256.Five.OrderService.Core.Handlers.ResultTypes;

namespace Ozon.Route256.Five.OrderService.Core.Handlers;


public interface IHandler<in TRequest, TResponse>
{
    Task<HandlerResult<TResponse>> Handle(TRequest request, CancellationToken token);
}

public interface IHandler<in TRequest>
{
    Task<HandlerResult> Handle(TRequest request, CancellationToken token);
}
