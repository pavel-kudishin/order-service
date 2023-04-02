using Ozon.Route256.Five.OrderService.Core.Handlers.OrderEvents.Dto;
using Ozon.Route256.Five.OrderService.Domain.BusinessObjects;

namespace Ozon.Route256.Five.OrderService.Core.Handlers.OrderEvents;

internal static class MappingExtensions
{
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
}