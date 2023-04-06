namespace Ozon.Route256.Five.OrderService.Core.Exceptions;

public sealed class InvalidConfigurationException: Exception
{
    public InvalidConfigurationException(string? message) : base(message)
    {
    }
}