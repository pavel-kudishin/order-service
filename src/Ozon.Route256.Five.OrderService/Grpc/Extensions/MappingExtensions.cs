using Google.Protobuf.WellKnownTypes;
using Ozon.Route256.Five.Orders.Grpc;
using Ozon.Route256.Five.OrderService.Core.BusinessObjects;
using Ozon.Route256.Five.OrderService.Core.Repository.Dto;

namespace Ozon.Route256.Five.OrderService.Grpc.Extensions;

public static class MappingExtensions
{
    public static Orders.Grpc.Address ToProtoAddress(this AddressBo address)
    {
        return new Orders.Grpc.Address()
        {
            Region = address.Region,
            City = address.City,
            Street = address.Street,
            Building = address.Building,
            Apartment = address.Apartment,
            Latitude = address.Latitude,
            Longitude = address.Longitude,
        };
    }

    public static Order ToProtoOrder(this OrderBo order)
    {
        return new Order()
        {
            Id = order.Id,
            GoodsCount = order.GoodsCount,
            TotalPrice = (double)order.TotalPrice,
            TotalWeight = (double)order.TotalWeight,
            OrderType = order.Source.ToString(),
            DateCreated = DateTime.SpecifyKind(order.DateCreated, DateTimeKind.Utc).ToTimestamp(),
            State = order.State.ToString(),
            CustomerName = order.Customer != null ? $"{order.Customer.LastName} {order.Customer.FirstName}" : null,
            DeliveryAddress = order.Address?.ToProtoAddress(),
            Phone = order.Phone
        };
    }

    public static CustomerDto ToCustomerDto(this Customer customer)
    {
        return new CustomerDto(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.MobileNumber,
            customer.Email,
            customer.DefaultAddress.ToAddressDto(),
            customer.Addresses.ToAddressesDto()
        );
    }

    public static AddressDto ToAddressDto(this Address address)
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

    public static CustomerDto[] ToCustomersDto(this IEnumerable<Customer> customers)
    {
        return customers.Select(c => c.ToCustomerDto()).ToArray();
    }

    public static AddressDto[] ToAddressesDto(this IEnumerable<Address> addresses)
    {
        return addresses.Select(c => c.ToAddressDto()).ToArray();
    }
}