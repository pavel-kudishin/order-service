namespace Ozon.Route256.Five.OrderService.Core.Repository.Dto;

public record AddressDto(
	int Id,
	string City,
	string Street,
	string Building,
	string Apartment,
	double Latitude,
	double Longitude,
	int RegionId);