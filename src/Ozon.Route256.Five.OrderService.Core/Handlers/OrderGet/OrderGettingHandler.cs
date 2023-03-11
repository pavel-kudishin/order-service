using Ozon.Route256.Five.OrderService.Core.BusinessObjects;
using Ozon.Route256.Five.OrderService.Core.Handlers.ResultTypes;
using Ozon.Route256.Five.OrderService.Core.Repository;
using Ozon.Route256.Five.OrderService.Core.Repository.Dto;
using Ozon.Route256.Five.OrderService.Core.Repository.Extensions;

namespace Ozon.Route256.Five.OrderService.Core.Handlers.OrderGet;

public class OrderGettingHandler : IOrderGettingHandler
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
        OrderDto? order = await _orderRepository.Find(request.OrderId, token);

        if (order is null)
        {
            return HandlerResult<OrderBo>.FromError(new OrderGettingException($"Order #{request.OrderId} not found"));
        }


        OrderBo orderBo = order.ToOrderBo();

        return HandlerResult<OrderBo>.FromValue(orderBo);
    }
}