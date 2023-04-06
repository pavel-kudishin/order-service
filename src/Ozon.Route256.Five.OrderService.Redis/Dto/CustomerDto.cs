namespace Ozon.Route256.Five.OrderService.Redis.Dto;

internal record CustomerDto(
    int Id,
    string FirstName,
    string LastName,
    string MobileNumber,
    string Email,
    AddressDto? DefaultAddress,
    AddressDto[]? Addresses);