using Microsoft.Extensions.Logging;

namespace Ozon.Route256.Five.OrderService.Core.Logging;

public static partial class LogExtensions
{
    [LoggerMessage(1, LogLevel.Information, "Получены новые данные из SD. Timestamp {Timestamp}")]
    public static partial void LogSdServiceResponseReceived(this ILogger logger, DateTime timestamp);

    [LoggerMessage(2, LogLevel.Error, "Order not found. OrderId {OrderId}")]
    public static partial void LogOrderNotFound(this ILogger logger, long orderId);

    [LoggerMessage(3, LogLevel.Error, "Customer not found. CustomerId {CustomerId}")]
    public static partial void LogCustomerNotFound(this ILogger logger, long customerId);

    [LoggerMessage(4, LogLevel.Error, "Region not found. Region {Region}")]
    public static partial void LogRegionNotFound(this ILogger logger, string region);

    [LoggerMessage(5, LogLevel.Error, "Invalid order. OrderId {OrderId}")]
    public static partial void LogInvalidOrder(this ILogger logger, long orderId);

    [LoggerMessage(6, LogLevel.Information, "Order published. OrderId {OrderId}")]
    public static partial void LogOrderPublished(this ILogger logger, long orderId);

    [LoggerMessage(7, LogLevel.Information, "Success subscribe to {Topic}")]
    public static partial void LogSuccessSubscribeToTopic(this ILogger logger, string topic);

    [LoggerMessage(
        8,
        LogLevel.Error,
        "Failed to handle message with key {MessageKey} in topic {Topic}")]
    public static partial void LogFailedToHandle(this ILogger logger, Exception e, string? messageKey, string topic);

    [LoggerMessage(
        9,
        LogLevel.Error,
        "Error in topic {Topic} during kafka consume")]
    public static partial void LogErrorInTopic(this ILogger logger, Exception e, string topic);

    [LoggerMessage(
        10,
        LogLevel.Error,
        "Не удалось связаться с SD")]
    public static partial void LogSdConnectionError(this ILogger logger, Exception e);

    [LoggerMessage(
        11,
        LogLevel.Error,
        "Error in {Topic} producer")]
    public static partial void LogTopicProducerError(this ILogger logger, Exception e, string topic);

}