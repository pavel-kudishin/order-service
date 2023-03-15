using Ozon.Route256.Five.LogisticsSimulator.Grpc;
using Ozon.Route256.Five.OrderService.Core;
using Ozon.Route256.Five.OrderService.Core.Handlers;
using Ozon.Route256.Five.OrderService.Core.Handlers.ResultTypes;

namespace Ozon.Route256.Five.OrderService.Infrastructure;

public class LogisticService: ILogisticService
{
    private readonly LogisticsSimulatorService.LogisticsSimulatorServiceClient _client;

    public LogisticService(
        LogisticsSimulatorService.LogisticsSimulatorServiceClient client)
    {
        _client = client;
    }

    public async Task<HandlerResult> OrderCancelAsync(long orderId, CancellationToken token)
    {
        Order clientRequest = new Order()
        {
            Id = orderId,
        };
        CancelResult result = await _client.OrderCancelAsync(clientRequest, cancellationToken: token);

        if (result.Success == false)
        {
            return HandlerResult.FromError(new HandlerException(result.Error));
        }

        return HandlerResult.Ok;
    }
}