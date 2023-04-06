namespace Ozon.Route256.Five.OrderService.Core.Handlers.PreOrders;

public interface INewOrdersKafkaPublisher
{
    Task PublishToKafka(NewOrderDto dto, CancellationToken token);
}