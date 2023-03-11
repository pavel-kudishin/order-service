namespace Ozon.Route256.Five.OrderService.Kafka;

public class InfrastructureKafkaException: Exception
{
    public InfrastructureKafkaException(string? message) : base(message)
    {
    }
}