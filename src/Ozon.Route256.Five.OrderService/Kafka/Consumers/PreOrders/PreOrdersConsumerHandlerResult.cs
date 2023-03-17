namespace Ozon.Route256.Five.OrderService.Kafka.Consumers.PreOrders;

public enum PreOrdersConsumerHandlerResult
{
    CustomerNotFound,
    RegionNotFound,
    InvalidOrder,
    Success
}