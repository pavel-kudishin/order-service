namespace Ozon.Route256.Five.OrderService.Kafka.Consumers.PreOrders;

public record PreOrderCustomerDto(
    int Id,
    PreOrderAddressDto Address);