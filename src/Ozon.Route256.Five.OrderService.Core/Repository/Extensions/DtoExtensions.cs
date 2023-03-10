using Ozon.Route256.Five.OrderService.Core.Repository.Dto;

namespace Ozon.Route256.Five.OrderService.Core.Repository.Extensions;

public static class DtoExtensions
{
    public static IEnumerable<int> CollectAddressIds(this IEnumerable<OrderDto> orders)
    {
        return orders.Select(o => o.AddressId);
    }
    public static IEnumerable<int> CollectAddressIds(this CustomerDto[] customers)
    {
        return customers.Select(c => c.AddressId)
            .Union(customers.SelectMany(c => c.Addresses));
    }
}