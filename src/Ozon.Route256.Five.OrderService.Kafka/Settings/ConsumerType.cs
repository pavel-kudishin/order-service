namespace Ozon.Route256.Five.OrderService.Kafka.Settings;

public class ConsumerType
{
    public static readonly ConsumerType PreOrder = new("PreOrderConsumer");

    public static readonly ConsumerType OrderEvents = new("OrderEventsConsumer");

    private ConsumerType(string name)
    {
        Name = name;
    }

    public string Name { get; }

}