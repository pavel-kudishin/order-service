using Ozon.Route256.Five.OrderService.Core.BusinessObjects;
using Ozon.Route256.Five.OrderService.Core.Handlers.ResultTypes;
using Ozon.Route256.Five.OrderService.Core.Repository;
using Ozon.Route256.Five.OrderService.Core.Repository.Dto;

namespace Ozon.Route256.Five.OrderService.Core.Handlers.OrderCancel;

public class OrderCancellationHandler : IOrderCancellationHandler
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
        OrderDto? order = await _orderRepository.Find(request.OrderId, token);

        if (order is null)
        {
            return HandlerResult.FromError(new OrderNotFoundException($"Order #{request.OrderId} not found"));
        }

        HandlerResult result = await _client.OrderCancelAsync(request.OrderId, token);

        if (result.Success == false)
        {
            return HandlerResult.FromError(new OrderCancellationException(result.Error.BusinessError));
        }

        order.State = OrderStateDto.Cancelled;

        await _orderRepository.Update(order, token);

        return HandlerResult.Ok;
    }
}