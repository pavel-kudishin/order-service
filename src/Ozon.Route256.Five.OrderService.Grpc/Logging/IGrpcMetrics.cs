namespace Ozon.Route256.Five.OrderService.Grpc.Logging;

public interface IGrpcMetrics
{
    void ResponseTime(long stopwatchElapsedMilliseconds, string contextMethod, bool isError);
}