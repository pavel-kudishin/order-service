using System.ComponentModel.DataAnnotations;

namespace Ozon.Route256.Five.OrderService.Rest.Dto;

public sealed class RegionDto
{
    [Required]
    public int Id { get; set; }

    [Required]
    public string? Name { get; set; }
}