using Ozon.Route256.Five.OrderService.Core.BusinessObjects;
using Ozon.Route256.Five.OrderService.Core.Handlers.ResultTypes;
using Ozon.Route256.Five.OrderService.Core.Repository;
using Ozon.Route256.Five.OrderService.Core.Repository.Dto;
using Ozon.Route256.Five.OrderService.Core.Repository.Extensions;

namespace Ozon.Route256.Five.OrderService.Core.Handlers.OrderStatusGet;

public class OrderStatusGettingHandler : IOrderStatusGettingHandler
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
        OrderDto? order = await _orderRepository.Find(request.OrderId, token);

        if (order is null)
        {
            return HandlerResult<OrderStateBo>.FromError(
                new OrderNotFoundException($"Order #{request.OrderId} not found"));
        }

        return HandlerResult<OrderStateBo>.FromValue(order.State.ToOrderStateBo());
    }
}