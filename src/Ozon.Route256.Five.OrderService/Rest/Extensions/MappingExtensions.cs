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
            DefaultAddress = customer.DefaultAddress?.ToAddressDto(),
            Addresses = customer.Addresses.ToAddressesDto(),
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
            request.Regions,
            request.StartDate,
            request.EndDate
        );
    }

    public static IOrdersGettingHandler.Request ToOrdersGettingHandlerRequest(
        this OrdersRequestDto request)
    {
        return new IOrdersGettingHandler.Request(
            request.Regions,
            request.Sources.ToSourcesDto(),
            request.PageNumber,
            request.ItemsPerPage,
            request.Direction.ToHandlersOrderingDirection()
        );
    }

    public static OrderSourceBo[]? ToSourcesDto(this OrderSourceDto[]? sources)
    {
        return sources?.Select(os => os.ToSourcesDto()).ToArray();
    }

    public static OrderSourceBo ToSourcesDto(this OrderSourceDto source)
    {
        return source switch
        {
            OrderSourceDto.WebSite => OrderSourceBo.WebSite,
            OrderSourceDto.Mobile => OrderSourceBo.Mobile,
            OrderSourceDto.Api => OrderSourceBo.Api,
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
            Region = response.Region,
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
            Region = address.Region,
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
            Name = region.Name,
            Warehouse = region.Warehouse?.ToWarehouseDto(),
        };
    }

    public static WarehouseDto ToWarehouseDto(this WarehouseBo region)
    {
        return new WarehouseDto()
        {
            Latitude = region.Latitude,
            Longitude = region.Longitude,
        };
    }

    public static OrderDto ToOrderDto(this OrderBo order)
    {
        return new OrderDto()
        {
            Id = order.Id,
            GoodsCount = order.GoodsCount,
            TotalPrice = order.TotalPrice,
            TotalWeight = order.TotalWeight,
            Source = order.Source.ToOrderSourceDto(),
            DateCreated = order.DateCreated,
            State = order.State.ToOrderStateDto(),
            Customer = order.Customer?.ToCustomerDto(),
            DeliveryAddress = order.Address?.ToAddressDto(),
            Phone = order.Phone,
        };
    }

    public static OrderSourceDto ToOrderSourceDto(this OrderSourceBo source)
    {
        return source switch
        {
            OrderSourceBo.WebSite => OrderSourceDto.WebSite,
            OrderSourceBo.Mobile => OrderSourceDto.Mobile,
            OrderSourceBo.Api => OrderSourceDto.Api,
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
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

    public static OrderStateDto ToOrderStateDto(this OrderStateBo source)
    {
        return source switch
        {
            OrderStateBo.Created => OrderStateDto.Created,
            OrderStateBo.SentToCustomer => OrderStateDto.SentToCustomer,
            OrderStateBo.Delivered => OrderStateDto.Delivered,
            OrderStateBo.Lost => OrderStateDto.Lost,
            OrderStateBo.Cancelled => OrderStateDto.Cancelled,
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
        };
    }
}