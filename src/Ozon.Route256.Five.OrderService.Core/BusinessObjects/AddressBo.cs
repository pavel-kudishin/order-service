namespace Ozon.Route256.Five.OrderService.Core.BusinessObjects;

public class AddressBo
{
    public int Id { get; init; }
    public string City { get; init; } = "";
    public string Street { get; init; } = "";
    public string Building { get; init; } = "";
    public string Apartment { get; init; } = "";
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public RegionBo? Region { get; init; }
}