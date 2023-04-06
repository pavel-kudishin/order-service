namespace Ozon.Route256.Five.OrderService.Domain.BusinessObjects;

public sealed record CustomerBo(
    int Id,
    string FirstName,
    string LastName,
    string MobileNumber,
    string Email,
    AddressBo? DefaultAddress,
    AddressBo[]? Addresses);
