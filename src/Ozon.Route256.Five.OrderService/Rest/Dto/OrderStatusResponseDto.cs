using System.ComponentModel.DataAnnotations;

namespace Ozon.Route256.Five.OrderService.Rest.Dto;

public sealed class OrderStatusResponseDto
{
    [Required]
    public string? StatusName { get; set; }
}