using Ozon.Route256.Five.OrderService.Core.Abstractions;
using Ozon.Route256.Five.OrderService.Core.Exceptions;
using Ozon.Route256.Five.OrderService.Domain.BusinessObjects;
using Ozon.Route256.Five.OrderService.Domain.Repository;

namespace Ozon.Route256.Five.OrderService.Core.Handlers.OrderCancel;

internal sealed class OrderCancellationHandler : IOrderCancellationHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogisticService _client;

    public OrderCancellationHandler(
        IOrderRepository orderRepository,
        ILogisticService client)
    {
        _orderRepository = orderRepository;
        _client = client;
    }

    public async Task<HandlerResult> Handle(IOrderCancellationHandler.Request request, CancellationToken token)
    {
        OrderBo? order = await _orderRepository.Find(request.OrderId, token);

        if (order is null)
        {
            return HandlerResult.FromError(new OrderNotFoundException($"Order #{request.OrderId} not found"));
        }

        HandlerResult result = await _client.OrderCancelAsync(request.OrderId, token);

        if (result.Success == false)
        {
            return HandlerResult.FromError(new OrderCancellationException(result.Error.BusinessError));
        }

        order.Cancel();

        await _orderRepository.Update(order, token);

        return HandlerResult.Ok;
    }
}