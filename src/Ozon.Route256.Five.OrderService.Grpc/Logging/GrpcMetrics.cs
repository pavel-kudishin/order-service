using System.Diagnostics.Metrics;
using Prometheus;

namespace Ozon.Route256.Five.OrderService.Grpc.Logging;

internal class GrpcMetrics : IGrpcMetrics
{
    private readonly Histogram _responseTimeHistogram =
        Metrics.CreateHistogram("ozon_grpc_response_time_ms", string.Empty, "methodName", "isError");

    public void ResponseTime(long duration, string methodName, bool isError)
    {
        _responseTimeHistogram
            .WithLabels(methodName, isError ? "1" : "0")
            .Observe(duration);
    }
}