namespace Ozon.Route256.Five.OrderService.Core.Handlers.PreOrders.Dto;

public record PreOrderAddressDto(
    string Region,
    string City,
    string Street,
    string Building,
    string Apartment,
    double Latitude,
    double Longitude);