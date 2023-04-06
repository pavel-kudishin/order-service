namespace Ozon.Route256.Five.OrderService.Kafka.Exceptions;

public sealed class InfrastructureKafkaException : Exception
{
    public InfrastructureKafkaException(string? message) : base(message)
    {
    }
}