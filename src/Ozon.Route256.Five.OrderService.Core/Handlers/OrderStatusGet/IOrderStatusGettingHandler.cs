using Ozon.Route256.Five.OrderService.Domain.BusinessObjects;

namespace Ozon.Route256.Five.OrderService.Core.Handlers.OrderStatusGet;

public interface IOrderStatusGettingHandler: IHandler<IOrderStatusGettingHandler.Request, OrderStateBo>
{
	public record Request(long OrderId);
}