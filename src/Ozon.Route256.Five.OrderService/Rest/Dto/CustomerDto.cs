using System.ComponentModel.DataAnnotations;

namespace Ozon.Route256.Five.OrderService.Rest.Dto;

public sealed class CustomerDto
{
    [Required]
    public int Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? MobileNumber { get; set; }

    public string? Email { get; set; }

    public AddressDto? Address { get; set; }

    public AddressDto[]? Addresses { get; set; }
}
