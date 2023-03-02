using System.ComponentModel.DataAnnotations;

namespace Ozon.Route256.Five.OrderService.Dto;

public sealed class AggregatedOrdersRequestDto
{
	/// <summary>
	/// Дата начала
	/// </summary>
	[Required]
	public DateTime StartDate { get; set; }

	/// <summary>
	/// Дата окончания
	/// </summary>
	public DateTime? EndDate { get; set; }

	/// <summary>
	/// ИД регионов для фильтрации
	/// </summary>
	public int[]? RegionIds { get; set; }
}