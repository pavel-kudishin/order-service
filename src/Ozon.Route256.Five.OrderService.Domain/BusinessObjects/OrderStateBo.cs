namespace Ozon.Route256.Five.OrderService.Domain.BusinessObjects;

public enum OrderStateBo
{
    Created,
    SentToCustomer,
    Delivered,
    Lost,
    Cancelled
}