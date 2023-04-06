namespace Ozon.Route256.Five.OrderService.Core.Handlers.PreOrders.Dto;

public record PreOrderDto(
    long Id,
    PreOrderSource Source,
    PreOrderCustomerDto Customer,
    IReadOnlyList<PreOrderGoodsDto> Goods);