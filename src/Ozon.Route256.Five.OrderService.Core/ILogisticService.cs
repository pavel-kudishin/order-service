using Ozon.Route256.Five.OrderService.Core.Handlers.ResultTypes;

namespace Ozon.Route256.Five.OrderService.Core;

public interface ILogisticService
{
    Task<HandlerResult> OrderCancelAsync(long orderId, CancellationToken token);
}