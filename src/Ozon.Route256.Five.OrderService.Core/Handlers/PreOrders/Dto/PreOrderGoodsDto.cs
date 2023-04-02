namespace Ozon.Route256.Five.OrderService.Core.Handlers.PreOrders.Dto;

public record PreOrderGoodsDto(
    long Id,
    string Name,
    int Quantity,
    decimal Price,
    uint Weight);