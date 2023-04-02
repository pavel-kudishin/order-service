namespace Ozon.Route256.Five.OrderService.Redis.Settings;

internal sealed class RedisSettings
{
    public RedisSettings()
    {
        Timeouts = new TimeoutSettings();
    }

    public string? ConnectionString { get; set; }

    public TimeoutSettings Timeouts { get; set; }
}