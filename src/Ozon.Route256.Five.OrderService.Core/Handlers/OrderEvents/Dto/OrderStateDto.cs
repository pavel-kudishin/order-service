namespace Ozon.Route256.Five.OrderService.Core.Handlers.OrderEvents.Dto;

public enum OrderStateDto
{
    Created,
    SentToCustomer,
    Delivered,
    Lost,
    Cancelled
}