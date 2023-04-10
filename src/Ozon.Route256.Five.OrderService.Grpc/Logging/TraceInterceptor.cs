using Grpc.Core.Interceptors;
using Grpc.Core;
using System.Diagnostics;
using Ozon.Route256.Five.OrderService.Core.Logging;

namespace Ozon.Route256.Five.OrderService.Grpc.Logging;

public class TraceInterceptor : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        using Activity activity = OrderActivitySourceConfig.OrderActivitySource
            .StartActivity(context.Method)!
            .AddTag("Now", DateTime.UtcNow);

        await context.WriteResponseHeadersAsync(
            new Metadata
            {
                {
                    "x-o3-trace-id", Activity.Current?.Id!
                }
            });
        return await base.UnaryServerHandler(request, context, continuation);
    }
}