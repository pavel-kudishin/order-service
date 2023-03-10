using Ozon.Route256.Five.OrderService.Core.BusinessObjects;
using Ozon.Route256.Five.OrderService.Core.Repository.Dto;

namespace Ozon.Route256.Five.OrderService.Core.Repository.Extensions;

public static class MappingExtensions
{
    public static CustomerBo ToCustomerBo(this CustomerDto customer, AddressBo? address, AddressBo[]? addresses)
    {
        return new CustomerBo()
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            MobileNumber = customer.MobileNumber,
            Email = customer.Email,
            Address = address,
            Addresses = addresses,
        };
    }

    public static AddressBo ToAddressBo(this AddressDto address, RegionBo region)
    {
        return new AddressBo()
        {
            Id = address.Id,
            Region = region,
            Street = address.Street,
            Building = address.Building,
            Apartment = address.Apartment,
            City = address.City,
            Latitude = address.Latitude,
            Longitude = address.Longitude,
        };
    }

    public static RegionBo ToRegionBo(this RegionDto region)
    {
        return new RegionBo()
        {
            Id = region.Id,
            Name = region.Name,
        };
    }

    public static OrderBo ToOrderBo(this OrderDto order, AddressBo? address, CustomerBo? customer)
    {
        return new OrderBo()
        {
            Id = order.Id,
            ArticlesCount = order.ArticlesCount,
            TotalPrice = order.TotalPrice,
            TotalWeight = order.TotalWeight,
            OrderType = order.OrderType.ToBoOrderType(),
            DateCreated = order.DateCreated,
            Status = order.Status,
            Customer = customer,
            Address = address,
            Phone = order.Phone

        };
    }

    public static OrderTypesBo ToBoOrderType(this OrderTypesDto orderType)
    {
        return orderType switch
        {
            OrderTypesDto.Pickup => OrderTypesBo.Pickup,
            OrderTypesDto.Delivery => OrderTypesBo.Delivery,
            _ => throw new ArgumentOutOfRangeException(nameof(orderType), orderType, null)
        };
    }

    public static OrderTypesDto ToDtoOrderType(this OrderTypesBo orderType)
    {
        return orderType switch
        {
            OrderTypesBo.Pickup => OrderTypesDto.Pickup,
            OrderTypesBo.Delivery => OrderTypesDto.Delivery,
            _ => throw new ArgumentOutOfRangeException(nameof(orderType), orderType, null)
        };
    }

    public static OrderTypesDto[]? ToDtoOrderTypes(this OrderTypesBo[]? orderTypes)
    {
        return orderTypes?.Select(o => o.ToDtoOrderType()).ToArray();
    }

    public static OrderingDirectionDto ToDtoDirection(this OrderingDirectionBo direction)
    {
        return direction switch
        {
            OrderingDirectionBo.Asc => OrderingDirectionDto.Asc,
            OrderingDirectionBo.Desc => OrderingDirectionDto.Desc,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }

    public static IEnumerable<CustomerBo> ToCustomersBo(
        this IEnumerable<CustomerDto> customers,
        IReadOnlyDictionary<int, AddressDto> addressesDictionary,
        IReadOnlyDictionary<int, RegionDto> regionsDictionary)
    {
        foreach (CustomerDto customer in customers)
        {
            CustomerBo customerBo = customer.ToCustomerBo(addressesDictionary, regionsDictionary);
            yield return customerBo;
        }
    }

    public static IEnumerable<OrderBo> ToOrdersBo(
        this IEnumerable<OrderDto> orders,
        IReadOnlyDictionary<int, AddressDto>? addressesDictionary,
        IReadOnlyDictionary<int, RegionDto> regionsDictionary,
        IReadOnlyDictionary<int, CustomerDto>? customersDictionary)
    {
        foreach (OrderDto order in orders)
        {
            OrderBo orderBo = order.ToOrderBo(addressesDictionary, regionsDictionary, customersDictionary);
            yield return orderBo;
        }
    }

    private static OrderBo ToOrderBo(
        this OrderDto order,
        IReadOnlyDictionary<int, AddressDto>? addressesDictionary,
        IReadOnlyDictionary<int, RegionDto> regionsDictionary,
        IReadOnlyDictionary<int, CustomerDto>? customersDictionary)
    {
        AddressBo? address = addressesDictionary != null
            ? GetAddressBo(order.AddressId, addressesDictionary, regionsDictionary)
            : null;

        CustomerBo? customer = customersDictionary != null
            ? GetCustomerBo(order.CustomerId, customersDictionary, addressesDictionary, regionsDictionary)
            : null;

        OrderBo orderBo = order.ToOrderBo(address, customer);
        return orderBo;
    }

    private static CustomerBo GetCustomerBo(int customerId,
        IReadOnlyDictionary<int, CustomerDto> customersDictionary,
        IReadOnlyDictionary<int, AddressDto>? addressesDictionary,
        IReadOnlyDictionary<int, RegionDto> regionsDictionary)
    {
        return customersDictionary[customerId].ToCustomerBo(addressesDictionary, regionsDictionary);
    }

    public static CustomerBo ToCustomerBo(
        this CustomerDto customer,
        IReadOnlyDictionary<int, AddressDto>? addressesDictionary,
        IReadOnlyDictionary<int, RegionDto> regionsDictionary)
    {
        if (addressesDictionary == null)
        {
            return customer.ToCustomerBo(address: null, addresses: null);
        }

        AddressBo? defaultAddressBo = GetAddressBo(customer.AddressId, addressesDictionary, regionsDictionary);

        AddressBo[] addresses = new AddressBo[customer.Addresses.Length];
        for (int i = 0; i < customer.Addresses.Length; i++)
        {
            AddressBo address = GetAddressBo(customer.Addresses[i], addressesDictionary, regionsDictionary);
            addresses[i] = address;
        }

        CustomerBo customerBo = customer.ToCustomerBo(defaultAddressBo, addresses);
        return customerBo;
    }

    public static AddressBo GetAddressBo(
        int addressId,
        IReadOnlyDictionary<int, AddressDto> addressesDictionary,
        IReadOnlyDictionary<int, RegionDto> regionsDictionary)
    {
        AddressDto addressDto = addressesDictionary[addressId];
        RegionDto regionDto = regionsDictionary[addressDto.RegionId];
        AddressBo addressBo = addressDto.ToAddressBo(regionDto.ToRegionBo());
        return addressBo;
    }

    public static RegionBo[] ToRegionsBo(
        this IEnumerable<RegionDto> regions)
    {
        return regions.Select(r => r.ToRegionBo()).ToArray();
    }

    public static IEnumerable<AggregatedOrdersResponseBo> ToAggregatedOrdersResponseBo(
        this AggregateOrdersDto[] array,
        Dictionary<int, RegionDto> regionsDictionary)
    {
        foreach (AggregateOrdersDto aggregateOrdersDto in array)
        {
            RegionDto regionDto = regionsDictionary[aggregateOrdersDto.RegionId];
            yield return new()
            {
                Region = regionDto.ToRegionBo(),
                TotalWeight = aggregateOrdersDto.TotalWeight,
                TotalOrdersPrice = aggregateOrdersDto.TotalOrdersPrice,
                CustomersCount = aggregateOrdersDto.CustomersCount,
                OrdersCount = aggregateOrdersDto.OrdersCount,
            };
        }
    }
}