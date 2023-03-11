namespace Ozon.Route256.Five.OrderService.Kafka.Consumers.PreOrders;

public record PreOrderGoodsDto(
    long Id,
    string Name,
    int Quantity,
    decimal Price,
    uint Weight);