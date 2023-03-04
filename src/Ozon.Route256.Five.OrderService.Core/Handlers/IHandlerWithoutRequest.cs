using Ozon.Route256.Five.OrderService.Core.Handlers.ResultTypes;

namespace Ozon.Route256.Five.OrderService.Core.Handlers;

public interface IHandlerWithoutRequest<TResponse>
{
    Task<HandlerResult<TResponse>> Handle(CancellationToken token);
}