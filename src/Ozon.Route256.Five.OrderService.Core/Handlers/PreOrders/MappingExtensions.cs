using Ozon.Route256.Five.OrderService.Core.Handlers.PreOrders.Dto;
using Ozon.Route256.Five.OrderService.Domain.BusinessObjects;

namespace Ozon.Route256.Five.OrderService.Core.Handlers.PreOrders;

internal static class MappingExtensions
{
    public static OrderBo ToOrderBo(this PreOrderDto order, CustomerBo customerBo)
    {
        decimal totalPrice = order.Goods.Sum(g => g.Price * g.Quantity);
        decimal totalWeight = order.Goods.Sum(g => g.Weight * g.Quantity);

        return new OrderBo(
            order.Id,
            order.Goods.Count(),
            totalPrice,
            totalWeight,
            order.Source.ToOrderSourceBo(),
            DateTime.UtcNow,
            OrderStateBo.Created,
            customerBo,
            order.Customer.Address.ToAddressBo(),
            customerBo.MobileNumber
        );
    }

    public static AddressBo ToAddressBo(this PreOrderAddressDto address)
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

    public static OrderSourceBo ToOrderSourceBo(this PreOrderSource source)
    {
        return source switch
        {
            PreOrderSource.WebSite => OrderSourceBo.WebSite,
            PreOrderSource.Mobile => OrderSourceBo.Mobile,
            PreOrderSource.Api => OrderSourceBo.Api,
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
        };
    }
}