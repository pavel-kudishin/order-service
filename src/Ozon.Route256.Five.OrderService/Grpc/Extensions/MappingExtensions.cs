using Google.Protobuf.WellKnownTypes;
using Ozon.Route256.Five.Orders.Grpc;
using Ozon.Route256.Five.OrderService.Core.BusinessObjects;

namespace Ozon.Route256.Five.OrderService.Grpc.Extensions;

public static class MappingExtensions
{
    public static Orders.Grpc.Address ToProtoAddress(this AddressBo address)
    {
        return new Orders.Grpc.Address()
        {
            Id = address.Id,
            Region = address.Region?.ToProtoRegion(),
            City = address.City,
            Street = address.Street,
            Building = address.Building,
            Apartment = address.Apartment,
            Latitude = address.Latitude,
            Longitude = address.Longitude,
        };
    }

    public static Region ToProtoRegion(this RegionBo region)
    {
        return new Region
        {
            Id = region.Id,
            Name = region.Name
        };
    }

    public static Order ToProtoOrder(this OrderBo order)
    {
        return new Order()
        {
            Id = order.Id,
            ArticlesCount = order.ArticlesCount,
            TotalPrice = order.TotalPrice,
            TotalWeight = order.TotalWeight,
            OrderType = order.OrderType.ToProtoOrderType(),
            DateCreated = order.DateCreated.ToTimestamp(),
            Status = order.Status,
            CustomerName = order.Customer != null ? $"{order.Customer.LastName} {order.Customer.FirstName}" : null,
            DeliveryAddress = order.Address?.ToProtoAddress(),
            Phone = order.Phone

        };
    }

    public static OrderTypes ToProtoOrderType(this OrderTypesBo orderType)
    {
        return orderType switch
        {
            OrderTypesBo.Pickup => OrderTypes.Pickup,
            OrderTypesBo.Delivery => OrderTypes.Delivery,
            _ => throw new ArgumentOutOfRangeException(nameof(orderType), orderType, null)
        };
    }
}