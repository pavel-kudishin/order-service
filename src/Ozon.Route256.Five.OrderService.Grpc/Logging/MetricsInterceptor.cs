using Grpc.Core.Interceptors;
using Grpc.Core;
using System.Diagnostics;

namespace Ozon.Route256.Five.OrderService.Grpc.Logging;

public class MetricsInterceptor : Interceptor
{
    private readonly IGrpcMetrics _grpcMetrics;

    public MetricsInterceptor(IGrpcMetrics grpcMetrics)
    {
        _grpcMetrics = grpcMetrics;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            TResponse result = await base.UnaryServerHandler(request, context, continuation);
            stopwatch.Stop();
            _grpcMetrics.ResponseTime(stopwatch.ElapsedMilliseconds, context.Method, false);

            return result;
        }
        catch
        {
            _grpcMetrics.ResponseTime(stopwatch.ElapsedMilliseconds, context.Method, true);

            throw;
        }
    }
}