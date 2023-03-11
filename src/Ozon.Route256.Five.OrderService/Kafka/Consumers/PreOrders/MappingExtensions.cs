using Ozon.Route256.Five.OrderService.Core.BusinessObjects;
using Ozon.Route256.Five.OrderService.Core.Repository.Dto;

namespace Ozon.Route256.Five.OrderService.Kafka.Consumers.PreOrders;

public static class MappingExtensions
{
    public static OrderDto ToOrderDto(this PreOrderDto order, CustomerDto customerDto)
    {
        decimal totalPrice = order.Goods.Sum(g => g.Price * g.Quantity);
        decimal totalWeight = order.Goods.Sum(g => g.Weight * g.Quantity);

        return new OrderDto(
            order.Id,
            order.Goods.Count(),
            totalPrice,
            totalWeight,
            order.Source.ToOrderSourceDto(),
            DateTime.UtcNow,
            order.Customer.Address.Region,
            OrderStateDto.Created,
            customerDto,
            order.Customer.Address.ToAddressDto(),
            customerDto.MobileNumber
        );
    }

    public static AddressDto ToAddressDto(this PreOrderAddressDto address)
    {
        return new AddressDto(
            address.City,
            address.Street,
            address.Building,
            address.Apartment,
            address.Latitude,
            address.Longitude,
            address.Region
        );
    }

    public static OrderSourceDto ToOrderSourceDto(this PreOrderSource source)
    {
        return source switch
        {
            PreOrderSource.WebSite => OrderSourceDto.WebSite,
            PreOrderSource.Mobile => OrderSourceDto.Mobile,
            PreOrderSource.Api => OrderSourceDto.Api,
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
        };
    }
}