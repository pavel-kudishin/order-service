namespace Ozon.Route256.Five.OrderService.Core;

public class InvalidConfigurationException: Exception
{
    public InvalidConfigurationException(string? message) : base(message)
    {
    }
}