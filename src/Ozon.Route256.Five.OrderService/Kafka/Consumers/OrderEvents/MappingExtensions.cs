namespace Ozon.Route256.Five.OrderService.Kafka.Consumers.OrderEvents;

public static class MappingExtensions
{
    public static Core.Repository.Dto.OrderStateDto ToOrderStateDto(this OrderStateDto source)
    {
        return source switch
        {
            OrderStateDto.Created => Core.Repository.Dto.OrderStateDto.Created,
            OrderStateDto.SentToCustomer => Core.Repository.Dto.OrderStateDto.SentToCustomer,
            OrderStateDto.Delivered => Core.Repository.Dto.OrderStateDto.Delivered,
            OrderStateDto.Lost => Core.Repository.Dto.OrderStateDto.Lost,
            OrderStateDto.Cancelled => Core.Repository.Dto.OrderStateDto.Cancelled,
            _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
        };
    }
}