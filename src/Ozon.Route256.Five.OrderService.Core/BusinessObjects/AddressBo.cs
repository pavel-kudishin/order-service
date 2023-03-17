namespace Ozon.Route256.Five.OrderService.Core.BusinessObjects;

public class AddressBo
{
    public string City { get; init; } = string.Empty;
    public string Street { get; init; } = string.Empty;
    public string Building { get; init; } = string.Empty;
    public string Apartment { get; init; } = string.Empty;
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string Region { get; init; } = string.Empty;
}