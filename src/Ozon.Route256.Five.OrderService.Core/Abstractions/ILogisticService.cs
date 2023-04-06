using Ozon.Route256.Five.OrderService.Core.Handlers;

namespace Ozon.Route256.Five.OrderService.Core.Abstractions;

public interface ILogisticService
{
    Task<HandlerResult> OrderCancelAsync(long orderId, CancellationToken token);
}