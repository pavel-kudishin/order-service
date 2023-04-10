using System.Diagnostics;

namespace Ozon.Route256.Five.OrderService.Core.Logging;

public static class OrderActivitySourceConfig
{
    public const string SOURCE_NAME = "Ozon.Route256.Five.OrderService.Core.Logging";

    public static readonly ActivitySource OrderActivitySource = new(SOURCE_NAME);
}