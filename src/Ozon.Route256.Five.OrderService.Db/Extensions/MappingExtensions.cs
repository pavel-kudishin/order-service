using Ozon.Route256.Five.OrderService.Db.Dto;
using Ozon.Route256.Five.OrderService.Domain.BusinessObjects;

namespace Ozon.Route256.Five.OrderService.Db.Extensions;

internal static class MappingExtensions
{
    public static AddressBo ToAddressBo(this AddressDto address)
    {
        return new AddressBo(
            address.City,
            address.Region,
            address.Building,
            address.Apartment,
            address.Latitude,
            address.Longitude,
            address.Region
        );
    }

    public static AddressBo[] ToAddressesBo(this AddressDto[] addresses)
    {
        return addresses.Select(a => a.ToAddressBo()).ToArray();
    }

    public static RegionBo ToRegionBo(this RegionDto region)
    {
        return new RegionBo(region.Name, region.Warehouse.ToWarehouseBo());
    }

    public static WarehouseBo ToWarehouseBo(this WarehouseDto region)
    {
        return new WarehouseBo(region.Latitude, region.Longitude);
    }

    public static OrderBo ToOrderBo(this OrderDto order, CustomerBo customerBo)
    {
        return new OrderBo(
            order.Id,
            order.GoodsCount,
            order.TotalPrice,
            order.TotalWeight,
            order.Source.OrderSourceBo(),
            order.DateCreated,
            order.State.ToOrderStateBo(),
            customerBo,
            order.Address?.ToAddressBo(),
            order.Phone
        );
    }

    public static OrderSourceBo OrderSourceBo(this OrderSourceDto source)
    {
        return source switch
        {
            OrderSourceDto.WebSite => Domain.BusinessObjects.OrderSourceBo.WebSite,
            OrderSourceDto.Mobile => Domain.BusinessObjects.OrderSourceBo.Mobile,
            OrderSourceDto.Api => Domain.BusinessObjects.OrderSourceBo.Api,
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
        };
    }

    public static OrderStateBo ToOrderStateBo(this OrderStateDto source)
    {
        return source switch
        {
            OrderStateDto.Created => OrderStateBo.Created,
            OrderStateDto.SentToCustomer => OrderStateBo.SentToCustomer,
            OrderStateDto.Delivered => OrderStateBo.Delivered,
            OrderStateDto.Lost => OrderStateBo.Lost,
            OrderStateDto.Cancelled => OrderStateBo.Cancelled,
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
        };
    }

    public static OrderingDirectionDto ToOrderingDirectionDto(this OrderingDirectionBo direction)
    {
        return direction switch
        {
            OrderingDirectionBo.Asc => OrderingDirectionDto.Asc,
            OrderingDirectionBo.Desc => OrderingDirectionDto.Desc,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }

    public static OrderSourceDto ToOrderSourceDto(this OrderSourceBo source)
    {
        return source switch
        {
            Domain.BusinessObjects.OrderSourceBo.WebSite => OrderSourceDto.WebSite,
            Domain.BusinessObjects.OrderSourceBo.Mobile => OrderSourceDto.Mobile,
            Domain.BusinessObjects.OrderSourceBo.Api => OrderSourceDto.Api,
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
        };
    }

    public static OrderBo[] ToOrdersBo(this IEnumerable<OrderDto> orders, CustomerBo[] customers)
    {
        OrderBo[] orderBos = orders.GroupJoin(customers,
                order => order.CustomerId,
                customer => customer.Id,
                (order, customersBo) => order.ToOrderBo(customersBo.First()))
            .ToArray();

        return orderBos;
    }

    public static RegionBo[] ToRegionsBo(
        this IEnumerable<RegionDto> regions)
    {
        return regions.Select(r => r.ToRegionBo()).ToArray();
    }

    public static AggregatedOrdersBo[] ToAggregatedOrdersBo(this AggregateOrdersDto[] array)
    {
        return array.Select(a => new AggregatedOrdersBo()
        {
            Region = a.Region,
            TotalWeight = a.TotalWeight,
            TotalOrdersPrice = a.TotalOrdersPrice,
            CustomersCount = a.CustomersCount,
            OrdersCount = a.OrdersCount,
        }).ToArray();
    }
}