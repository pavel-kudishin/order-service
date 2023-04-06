namespace Ozon.Route256.Five.OrderService.Domain.BusinessObjects;

public sealed record AddressBo(
    string City,
    string Street,
    string Building,
    string Apartment,
    double Latitude,
    double Longitude,
    string Region);