namespace Ozon.Route256.Five.OrderService.Db.Settings;

public sealed class PostgresSettings
{
    public string? Db { get; set; }
    public string? Login { get; set; }
    public string? Password { get; set; }
    public string? Cluster { get; set; }
}