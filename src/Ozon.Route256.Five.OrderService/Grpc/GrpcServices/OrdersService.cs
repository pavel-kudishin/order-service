using Grpc.Core;
using Ozon.Route256.Five.Orders.Grpc;
using Ozon.Route256.Five.OrderService.Core.BusinessObjects;
using Ozon.Route256.Five.OrderService.Core.Handlers.OrderGet;
using Ozon.Route256.Five.OrderService.Core.Handlers.ResultTypes;
using Ozon.Route256.Five.OrderService.Grpc.Extensions;

namespace Ozon.Route256.Five.OrderService.Grpc.GrpcServices;

public sealed class OrdersService : Orders.Grpc.Orders.OrdersBase
{
    private readonly IOrderGettingHandler _handler;

    public OrdersService(IOrderGettingHandler handler)
    {
        _handler = handler;
    }

    public override async Task<Order> GetOrder(GetOrderRequest request, ServerCallContext context)
    {
        IOrderGettingHandler.Request handlerRequest = new(request.OrderId);
        HandlerResult<OrderBo> result = await _handler.Handle(handlerRequest, context.CancellationToken);

        if (result.Success == false)
        {
            throw new RpcException(new Status(StatusCode.NotFound, result.Error.BusinessError));
        }

        return result.Value.ToProtoOrder();
    }
}