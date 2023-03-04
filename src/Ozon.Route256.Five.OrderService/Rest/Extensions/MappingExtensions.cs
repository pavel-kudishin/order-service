using Ozon.Route256.Five.OrderService.Core.BusinessObjects;
using Ozon.Route256.Five.OrderService.Core.Handlers.AggregateOrders;
using Ozon.Route256.Five.OrderService.Core.Handlers.OrdersByCustomerGet;
using Ozon.Route256.Five.OrderService.Core.Handlers.OrdersGet;
using Ozon.Route256.Five.OrderService.Rest.Dto;

namespace Ozon.Route256.Five.OrderService.Rest.Extensions;

public static class MappingExtensions
{
    public static CustomerDto ToCustomerDto(this CustomerBo customer)
    {
        return new CustomerDto()
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            MobileNumber = customer.MobileNumber,
            Email = customer.Email,
            Address = customer.Address?.ToAddressDto(),
            Addresses = customer.Addresses?.ToAddressesDto(),
        };
    }

    public static CustomerDto[]? ToCustomersDto(this CustomerBo[]? customers)
    {
        return customers?.Select(c => c.ToCustomerDto()).ToArray();
    }

    public static RegionDto[]? ToRegionsDto(this RegionBo[]? regions)
    {
        return regions?.Select(r => r.ToRegionDto()).ToArray();
    }

    public static AddressDto[]? ToAddressesDto(this AddressBo[]? addresses)
    {
        return addresses?.Select(a => a.ToAddressDto()).ToArray();
    }

    public static OrderDto[]? ToOrdersDto(this OrderBo[]? orders)
    {
        return orders?.Select(a => a.ToOrderDto()).ToArray();
    }

    public static AggregatedOrdersResponseDto[]? ToAggregatedOrdersResponseDto(
        this AggregatedOrdersResponseBo[]? responses)
    {
        return responses?.Select(r => r.ToAggregatedOrdersResponseDto()).ToArray();
    }

    public static IOrderAggregationHandler.Request ToOrderAggregationHandlerRequestBo(
        this AggregatedOrdersRequestDto request)
    {
        return new IOrderAggregationHandler.Request(
            request.RegionIds,
            request.StartDate,
            request.EndDate
        );
    }

    public static IOrdersGettingHandler.Request ToOrdersGettingHandlerRequest(
        this OrdersRequestDto request)
    {
        return new IOrdersGettingHandler.Request(
            request.RegionIds,
            request.OrderTypes?.Select(t => t.ToHandlersOrderType()).ToArray(),
            request.PageNumber,
            request.ItemsPerPage,
            request.Direction.ToHandlersOrderingDirection()
        );
    }

    public static IOrdersByCustomerGettingHandler.Request ToOrdersByCustomerGettingHandlerRequest(
        this OrdersByCustomerRequestDto request)
    {
        return new IOrdersByCustomerGettingHandler.Request(
            request.CustomerId,
            request.StartDate,
            request.EndDate,
            request.PageNumber,
            request.ItemsPerPage
        );
    }

    public static AggregatedOrdersResponseDto ToAggregatedOrdersResponseDto(
        this AggregatedOrdersResponseBo response)
    {
        return new AggregatedOrdersResponseDto()
        {
            Region = response.Region?.ToRegionDto(),
            TotalWeight = response.TotalWeight,
            CustomersCount = response.CustomersCount,
            OrdersCount = response.OrdersCount,
            TotalOrdersPrice = response.TotalOrdersPrice,
        };
    }


    public static AddressDto ToAddressDto(this AddressBo address)
    {
        return new AddressDto()
        {
            Id = address.Id,
            Region = address.Region?.ToRegionDto(),
            Street = address.Street,
            Building = address.Building,
            Apartment = address.Apartment,
            City = address.City,
            Latitude = address.Latitude,
            Longitude = address.Longitude,
        };
    }

    public static RegionDto ToRegionDto(this RegionBo region)
    {
        return new RegionDto()
        {
            Id = region.Id,
            Name = region.Name,
        };
    }

    public static OrderDto ToOrderDto(this OrderBo order)
    {
        return new OrderDto()
        {
            Id = order.Id,
            ArticlesCount = order.ArticlesCount,
            TotalPrice = order.TotalPrice,
            TotalWeight = order.TotalWeight,
            OrderType = order.OrderType.ToDtoOrderType(),
            DateCreated = order.DateCreated,
            Status = order.Status,
            Customer = order.Customer?.ToCustomerDto(),
            DeliveryAddress = order.Address?.ToAddressDto(),
            Phone = order.Phone,
        };
    }

    private static OrderTypesDto ToDtoOrderType(this OrderTypesBo orderType)
    {
        return orderType switch
        {
            OrderTypesBo.Pickup => OrderTypesDto.Pickup,
            OrderTypesBo.Delivery => OrderTypesDto.Delivery,
            _ => throw new ArgumentOutOfRangeException(nameof(orderType), orderType, null)
        };
    }

    private static OrderTypesBo ToHandlersOrderType(this OrderTypesDto orderType)
    {
        return orderType switch
        {
            OrderTypesDto.Pickup => OrderTypesBo.Pickup,
            OrderTypesDto.Delivery => OrderTypesBo.Delivery,
            _ => throw new ArgumentOutOfRangeException(nameof(orderType), orderType, null)
        };
    }

    private static OrderingDirectionBo ToHandlersOrderingDirection(this OrderingDirectionDto direction)
    {
        return direction switch
        {
            OrderingDirectionDto.Asc => OrderingDirectionBo.Asc,
            OrderingDirectionDto.Desc => OrderingDirectionBo.Desc,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };
    }
}