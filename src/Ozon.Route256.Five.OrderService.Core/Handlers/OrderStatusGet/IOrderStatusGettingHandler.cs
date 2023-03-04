namespace Ozon.Route256.Five.OrderService.Core.Handlers.OrderStatusGet;

public interface IOrderStatusGettingHandler: IHandler<IOrderStatusGettingHandler.Request, string>
{
	public record Request(long OrderId);
}