namespace Ozon.Route256.Five.OrderService.Kafka.Consumers.OrderEvents;

public record OrderEventsChangedDto (
    long Id,
    OrderStateDto NewState,
    DateTimeOffset ChangedAt);