namespace Ozon.Route256.Five.OrderService.Kafka.Consumers.OrderEvents;

public record OrderEventsChangedDto (
    long OrderId,
    OrderStateDto NewState,
    DateTimeOffset ChangedAt);