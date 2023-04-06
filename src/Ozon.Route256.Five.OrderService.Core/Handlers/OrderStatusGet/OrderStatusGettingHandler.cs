using Ozon.Route256.Five.OrderService.Core.Exceptions;
using Ozon.Route256.Five.OrderService.Domain.BusinessObjects;
using Ozon.Route256.Five.OrderService.Domain.Repository;

namespace Ozon.Route256.Five.OrderService.Core.Handlers.OrderStatusGet;

internal sealed class OrderStatusGettingHandler : IOrderStatusGettingHandler
{
    private readonly IOrderRepository _orderRepository;

    public OrderStatusGettingHandler(
        IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<HandlerResult<OrderStateBo>> Handle(
        IOrderStatusGettingHandler.Request request, CancellationToken token)
    {
        OrderBo? order = await _orderRepository.Find(request.OrderId, token);

        if (order is null)
        {
            return HandlerResult<OrderStateBo>.FromError(
                new OrderNotFoundException($"Order #{request.OrderId} not found"));
        }

        return HandlerResult<OrderStateBo>.FromValue(order.State);
    }
}