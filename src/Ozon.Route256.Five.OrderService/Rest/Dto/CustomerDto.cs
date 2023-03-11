using System.ComponentModel.DataAnnotations;

namespace Ozon.Route256.Five.OrderService.Rest.Dto;

public sealed class CustomerDto
{
    [Required]
    public int Id { get; init; }

    public string? FirstName { get; init; }

    public string? LastName { get; init; }

    public string? MobileNumber { get; init; }

    public string? Email { get; init; }

    public AddressDto? DefaultAddress { get; init; }

    public AddressDto[]? Addresses { get; init; }
}
