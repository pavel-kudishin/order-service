namespace Ozon.Route256.Five.OrderService.Core.Repository.Dto;

public record AddressDto(
	string City,
	string Street,
	string Building,
	string Apartment,
	double Latitude,
	double Longitude,
	string Region);