using Google.Protobuf.WellKnownTypes;
using Ozon.Route256.Five.Orders.Grpc;
using Ozon.Route256.Five.OrderService.Domain.BusinessObjects;

namespace Ozon.Route256.Five.OrderService.Grpc.Extensions;

internal static class MappingExtensions
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
            DateCreated = order.DateCreated.ToTimestamp(),
            State = order.State.ToString(),
            CustomerName = $"{order.Customer.LastName} {order.Customer.FirstName}",
            DeliveryAddress = order.Address?.ToProtoAddress(),
            Phone = order.Phone
        };
    }

    public static CustomerBo ToCustomerBo(this Customer customer)
    {
        return new CustomerBo(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.MobileNumber,
            customer.Email,
            customer.DefaultAddress.ToAddressBo(),
            customer.Addresses.ToAddressesBo()
        );
    }

    public static AddressBo ToAddressBo(this Address address)
    {
        return new AddressBo(
            address.City,
            address.Street,
            address.Building,
            address.Apartment,
            address.Latitude,
            address.Longitude,
            address.Region
        );
    }

    public static CustomerBo[] ToCustomersBo(this IEnumerable<Customer> customers)
    {
        return customers.Select(c => c.ToCustomerBo()).ToArray();
    }

    public static AddressBo[] ToAddressesBo(this IEnumerable<Address> addresses)
    {
        return addresses.Select(c => c.ToAddressBo()).ToArray();
    }
}