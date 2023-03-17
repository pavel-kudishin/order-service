namespace Ozon.Route256.Five.OrderService.Kafka.Consumers.OrderEvents;

public enum OrderStateDto
{
    Created,
    SentToCustomer,
    Delivered,
    Lost,
    Cancelled
}