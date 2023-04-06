namespace Ozon.Route256.Five.OrderService.Kafka.Producers.NewOrders;

public sealed class NewOrderSettings
{
    public const string SECTIONS = "Kafka:Producer:NewOrdersProducer";
    public string Topic { get; set; } = string.Empty;
}