using System.ComponentModel.DataAnnotations;

namespace Ozon.Route256.Five.OrderService.Dto;

public sealed class OrdersByCustomerRequestDto
{
	/// <summary>
	/// ИД клиента
	/// </summary>
	[Required]
	public int CustomerId { get; set; }

	/// <summary>
	/// Дата начала
	/// </summary>
	public DateTime? StartDate { get; set; }

	/// <summary>
	/// Дата окончания
	/// </summary>
	public DateTime? EndDate { get; set; }

	/// <summary>
	/// Номер запрашиваемой страницы. Нумерация начинается с 0
	/// </summary>
	[Range(0, int.MaxValue)]
	[Required]
	public int PageNumber { get; set; }

	/// <summary>
	/// Число элементов на странице
	/// </summary>
	[Range(1, 100)]
	[Required]
	public int ItemsPerPage { get; set; }
}