namespace Ozon.Route256.Five.OrderService.Core.Repository.Dto;

public record OrderDto(
    long Id,
    int GoodsCount,
    decimal TotalPrice,
    decimal TotalWeight,
    OrderSourceDto Source,
    DateTime DateCreated,
    string Region,
    OrderStateDto State,
    CustomerDto Customer,
    AddressDto Address,
    string Phone
);