using Ozon.Route256.Five.OrderService.Domain.BusinessObjects;
using Ozon.Route256.Five.OrderService.Redis.Dto;

namespace Ozon.Route256.Five.OrderService.Redis.Extensions;

internal static class MappingExtensions
{
    public static CustomerBo ToCustomerBo(this CustomerDto customer)
    {
        return new CustomerBo(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.MobileNumber,
            customer.Email,
            customer.DefaultAddress?.ToAddressBo(),
            customer.Addresses?.ToAddressesBo()
        );
    }

    public static AddressBo ToAddressBo(this AddressDto address)
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

    public static AddressBo[] ToAddressesBo(this AddressDto[] addresses)
    {
        return addresses.Select(a => a.ToAddressBo()).ToArray();
    }

    public static CustomerBo[] ToCustomersBo(this IEnumerable<CustomerDto> customers)
    {
        return customers.Select(c => c.ToCustomerBo()).ToArray();
    }

    public static CustomerDto ToCustomerDto(this CustomerBo customer)
    {
        return new CustomerDto(
            customer.Id,
            customer.FirstName,
            customer.LastName,
            customer.MobileNumber,
            customer.Email,
            customer.DefaultAddress?.ToAddressDto(),
            customer.Addresses?.ToAddressesDto()
        );
    }

    public static AddressDto ToAddressDto(this AddressBo address)
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

    public static AddressDto[] ToAddressesDto(this AddressBo[] addresses)
    {
        return addresses.Select(a => a.ToAddressDto()).ToArray();
    }

    public static CustomerDto[] ToCustomersDto(this IEnumerable<CustomerBo> customers)
    {
        return customers.Select(c => c.ToCustomerDto()).ToArray();
    }
}