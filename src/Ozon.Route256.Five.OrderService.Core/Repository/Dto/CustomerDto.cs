namespace Ozon.Route256.Five.OrderService.Core.Repository.Dto;

public record CustomerDto(
	int Id,
	string FirstName,
	string LastName,
	string MobileNumber,
	string Email,
	int AddressId,
	int[] Addresses);