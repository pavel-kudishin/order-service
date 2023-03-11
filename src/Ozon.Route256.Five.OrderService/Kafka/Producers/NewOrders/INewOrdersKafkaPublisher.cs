namespace Ozon.Route256.Five.OrderService.Kafka.Producers.NewOrders;

public interface INewOrdersKafkaPublisher
{
    Task PublishToKafka(NewOrderDto dto, CancellationToken token);
}