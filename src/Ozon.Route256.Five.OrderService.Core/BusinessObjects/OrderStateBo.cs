namespace Ozon.Route256.Five.OrderService.Core.BusinessObjects;

public enum OrderStateBo
{
    Created,
    SentToCustomer,
    Delivered,
    Lost,
    Cancelled
}