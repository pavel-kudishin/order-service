namespace Ozon.Route256.Five.OrderService.Core.BusinessObjects;

public class AddressBo
{
    public int Id { get; init; }
    public string City { get; init; } = string.Empty;
    public string Street { get; init; } = string.Empty;
    public string Building { get; init; } = string.Empty;
    public string Apartment { get; init; } = string.Empty;
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public RegionBo? Region { get; init; }
}