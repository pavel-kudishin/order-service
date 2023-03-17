namespace Ozon.Route256.Five.OrderService.Core.Redis.Settings;

public class RedisSettings
{
    public RedisSettings()
    {
        Timeouts = new TimeoutSettings();
    }

    public string? ConnectionString { get; set; }

    public TimeoutSettings Timeouts { get; set; }
}