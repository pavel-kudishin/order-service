namespace Ozon.Route256.Five.OrderService.Redis.Settings;

internal sealed class TimeoutSettings
{
    public TimeSpan Customer { get; set; }

    public TimeSpan Customers { get; set; }
}