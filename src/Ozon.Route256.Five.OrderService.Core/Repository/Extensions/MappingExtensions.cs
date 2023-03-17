using Ozon.Route256.Five.OrderService.Core.BusinessObjects;
using Ozon.Route256.Five.OrderService.Core.Repository.Dto;

namespace Ozon.Route256.Five.OrderService.Core.Repository.Extensions;

public static class MappingExtensions
{
    public static CustomerBo ToCustomerBo(this CustomerDto customer)
    {
        return new CustomerBo()
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            MobileNumber = customer.MobileNumber,
            Email = customer.Email,
            DefaultAddress = customer.DefaultAddress.ToAddressBo(),
            Addresses = customer.Addresses.ToAddressesBo(),
        };
    }

    public static AddressBo ToAddressBo(this AddressDto address)
    {
        return new AddressBo()
        {
            Region = address.Region,
            Street = address.Street,
            Building = address.Building,
            Apartment = address.Apartment,
            City = address.City,
            Latitude = address.Latitude,
            Longitude = address.Longitude,
        };
    }

    public static AddressBo[] ToAddressesBo(this AddressDto[] addresses)
    {
        return addresses.Select(a => a.ToAddressBo()).ToArray();
    }

    public static RegionBo ToRegionBo(this RegionDto region)
    {
        return new RegionBo()
        {
            Name = region.Name,
            Warehouse = region.Warehouse.ToWarehouseBo(),
        };
    }

    public static WarehouseBo ToWarehouseBo(this WarehouseDto region)
    {
        return new WarehouseBo()
        {
            Latitude = region.Latitude,
            Longitude = region.Longitude,
        };
    }

    public static OrderBo ToOrderBo(this OrderDto order, CustomerDto? customerDto)
    {
        return new OrderBo()
        {
            Id = order.Id,
            GoodsCount = order.GoodsCount,
            TotalPrice = order.TotalPrice,
            TotalWeight = order.TotalWeight,
            Source = order.Source.OrderSourceBo(),
            DateCreated = order.DateCreated,
            State = order.State.ToOrderStateBo(),
            Customer = customerDto?.ToCustomerBo(),
            Address = order.Address?.ToAddressBo(),
            Phone = order.Phone

        };
    }

    public static OrderSourceBo OrderSourceBo(this OrderSourceDto source)
    {
        return source switch
        {
            OrderSourceDto.WebSite => BusinessObjects.OrderSourceBo.WebSite,
            OrderSourceDto.Mobile => BusinessObjects.OrderSourceBo.Mobile,
            OrderSourceDto.Api => BusinessObjects.OrderSourceBo.Api,
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
            BusinessObjects.OrderSourceBo.WebSite => OrderSourceDto.WebSite,
            BusinessObjects.OrderSourceBo.Mobile => OrderSourceDto.Mobile,
            BusinessObjects.OrderSourceBo.Api => OrderSourceDto.Api,
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
        };
    }

    public static OrderSourceDto[]? ToOrderSourcesDto(this IEnumerable<OrderSourceBo>? sources)
    {
        return sources?.Select(c => c.ToOrderSourceDto()).ToArray();
    }

    public static CustomerBo[] ToCustomersBo(
        this IEnumerable<CustomerDto> customers)
    {
        return customers.Select(c => c.ToCustomerBo()).ToArray();
    }

    public static OrderBo[] ToOrdersBo(this IEnumerable<OrderDto> orders)
    {
        return orders.Select(o => o.ToOrderBo(null)).ToArray();
    }
    public static OrderBo[] ToOrdersBo(this IEnumerable<OrderDto> orders, CustomerDto[] customers)
    {
        Dictionary<int, CustomerDto> dictionary = customers.Distinct().ToDictionary(k => k.Id);
        return orders.Select(order =>
        {
            dictionary.TryGetValue(order.CustomerId, out CustomerDto? customer);
            return order.ToOrderBo(customer);
        }).ToArray();
    }

    public static RegionBo[] ToRegionsBo(
        this IEnumerable<RegionDto> regions)
    {
        return regions.Select(r => r.ToRegionBo()).ToArray();
    }

    public static AggregatedOrdersResponseBo[] ToAggregatedOrdersResponseBo(this AggregateOrdersDto[] array)
    {
        return array.Select(a => new AggregatedOrdersResponseBo()
        {
            Region = a.Region,
            TotalWeight = a.TotalWeight,
            TotalOrdersPrice = a.TotalOrdersPrice,
            CustomersCount = a.CustomersCount,
            OrdersCount = a.OrdersCount,
        }).ToArray();
    }
}