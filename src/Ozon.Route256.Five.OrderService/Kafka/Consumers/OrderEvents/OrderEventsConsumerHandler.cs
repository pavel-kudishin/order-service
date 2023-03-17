using Ozon.Route256.Five.OrderService.Core.Repository;
using Ozon.Route256.Five.OrderService.Core.Repository.Dto;
using Ozon.Route256.Five.OrderService.Kafka.Consumers.BackgroundConsumer;

namespace Ozon.Route256.Five.OrderService.Kafka.Consumers.OrderEvents;

public class OrderEventsConsumerHandler
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
        OrderDto? orderDto = await _orderRepository.Find(message.OrderId, token);

        if (orderDto == null)
        {
            _logger.LogError($"Order {message.OrderId} not found");
            return OrderEventsConsumerHandlerResult.NotFound;
        }

        OrderDto updatedOrder = orderDto with
        {
            State = message.NewState.ToOrderStateDto(),
        };

        await _orderRepository.Update(updatedOrder, token);

        return OrderEventsConsumerHandlerResult.Success;
    }
}