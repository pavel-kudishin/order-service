namespace Ozon.Route256.Five.OrderService.Core.Handlers.OrderEvents.Dto;

public record OrderEventsChangedDto(
    long Id,
    OrderStateDto NewState,
    DateTimeOffset ChangedAt);