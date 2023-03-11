using System.ComponentModel.DataAnnotations;

namespace Ozon.Route256.Five.OrderService.Rest.Dto;

public sealed class AggregatedOrdersRequestDto
{
    /// <summary>
    /// Дата начала
    /// </summary>
    [Required]
    public DateTime StartDate { get; init; }

    /// <summary>
    /// Дата окончания
    /// </summary>
    public DateTime? EndDate { get; init; }

    /// <summary>
    /// Регионы для фильтрации
    /// </summary>
    public string[]? Regions { get; init; }
}