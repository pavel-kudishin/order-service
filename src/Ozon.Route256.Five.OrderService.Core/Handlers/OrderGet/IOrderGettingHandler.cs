using Ozon.Route256.Five.OrderService.Core.BusinessObjects;

namespace Ozon.Route256.Five.OrderService.Core.Handlers.OrderGet;

public interface IOrderGettingHandler: IHandler<IOrderGettingHandler.Request, OrderBo>
{
	public record Request(long OrderId);
}