namespace Ozon.Route256.Five.OrderService.Db.Dto;

internal enum OrderStateDto
{
    Created,
    SentToCustomer,
    Delivered,
    Lost,
    Cancelled
}