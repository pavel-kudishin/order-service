namespace Ozon.Route256.Five.OrderService.Core.Repository.Dto;

public record OrderDto(
    long Id,
    int ArticlesCount,
    decimal TotalPrice,
    decimal TotalWeight,
    OrderTypesDto OrderType,
    DateTime DateCreated,
    int RegionId,
    string Status,
    int CustomerId,
    int AddressId,
    string Phone
);