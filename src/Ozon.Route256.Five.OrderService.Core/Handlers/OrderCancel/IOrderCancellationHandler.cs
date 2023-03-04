namespace Ozon.Route256.Five.OrderService.Core.Handlers.OrderCancel;

public interface IOrderCancellationHandler: IHandler<IOrderCancellationHandler.Request>
{
    public record Request(long OrderId);
}