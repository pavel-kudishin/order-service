namespace Ozon.Route256.Five.OrderService.Kafka.Producers;

public interface IKafkaProducer
{
    Task SendMessage(string key, string value, string topic, CancellationToken token);
}