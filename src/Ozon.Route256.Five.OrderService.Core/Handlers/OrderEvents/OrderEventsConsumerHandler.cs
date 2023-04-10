using Microsoft.Extensions.Logging;
using Ozon.Route256.Five.OrderService.Core.Handlers.OrderEvents.Dto;
using Ozon.Route256.Five.OrderService.Core.Logging;
using Ozon.Route256.Five.OrderService.Domain.BusinessObjects;
using Ozon.Route256.Five.OrderService.Domain.Repository;

namespace Ozon.Route256.Five.OrderService.Core.Handlers.OrderEvents;

public sealed class OrderEventsConsumerHandler
    : IKafkaConsumerHandler<string, OrderEventsChangedDto, OrderEventsConsumerHandlerResult>
{
    private readonly ILogger<OrderEventsConsumerHandler> _logger;
    private readonly IOrderRepository _orderRepository;

    public OrderEventsConsumerHandler(
        ILogger<OrderEventsConsumerHandler> logger,
        IOrderRepository orderRepository)
    {
        _logger = logger;
        _orderRepository = orderRepository;
    }

    public async Task<OrderEventsConsumerHandlerResult> Handle(
        string key, OrderEventsChangedDto message, CancellationToken token)
    {
        OrderBo? order = await _orderRepository.Find(message.Id, token);

        if (order == null)
        {
            _logger.LogOrderNotFound(message.Id);
            return OrderEventsConsumerHandlerResult.NotFound;
        }

        order.SetState(message.NewState.ToOrderStateBo());

        await _orderRepository.Update(order, token);

        return OrderEventsConsumerHandlerResult.Success;
    }
}