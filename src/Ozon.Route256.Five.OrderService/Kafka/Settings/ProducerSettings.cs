using Confluent.Kafka;

namespace Ozon.Route256.Five.OrderService.Kafka.Settings;

public class ProducerSettings
{
    public string? Topic { get; set; }

    public Acks Acks { get; set; } = Acks.Leader;

    public bool EnableIdempotence { get; set; } = false;
}