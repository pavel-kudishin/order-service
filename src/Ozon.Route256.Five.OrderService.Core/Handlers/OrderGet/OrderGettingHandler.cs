using Ozon.Route256.Five.OrderService.Domain.BusinessObjects;
using Ozon.Route256.Five.OrderService.Domain.Repository;

namespace Ozon.Route256.Five.OrderService.Core.Handlers.OrderGet;

internal sealed class OrderGettingHandler : IOrderGettingHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICustomerRepository _customerRepository;

    public OrderGettingHandler(
        IOrderRepository orderRepository,
        ICustomerRepository customerRepository)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
    }

    public async Task<HandlerResult<OrderBo>> Handle(IOrderGettingHandler.Request request, CancellationToken token)
    {
        OrderBo? order = await _orderRepository.Find(request.OrderId, token);

        if (order is null)
        {
            return HandlerResult<OrderBo>.FromError(new OrderGettingException($"Order #{request.OrderId} not found"));
        }

        return HandlerResult<OrderBo>.FromValue(order);
    }
}