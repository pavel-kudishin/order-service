using Ozon.Route256.Five.OrderService.Core.BusinessObjects;

namespace Ozon.Route256.Five.OrderService.Kafka.Consumers.PreOrders;

public record PreOrderDto(
    long Id,
    PreOrderSource Source,
    PreOrderCustomerDto Customer,
    IEnumerable<PreOrderGoodsDto> Goods);