using System.ComponentModel.DataAnnotations;

namespace Ozon.Route256.Five.OrderService.Rest.Dto;

public sealed class OrdersByCustomerRequestDto
{
    /// <summary>
    /// ИД клиента
    /// </summary>
    [Required]
    public int CustomerId { get; init; }

    /// <summary>
    /// Дата начала
    /// </summary>
    public DateTime? StartDate { get; init; }

    /// <summary>
    /// Дата окончания
    /// </summary>
    public DateTime? EndDate { get; init; }

    /// <summary>
    /// Номер запрашиваемой страницы. Нумерация начинается с 0
    /// </summary>
    [Range(0, int.MaxValue)]
    [Required]
    public int PageNumber { get; init; }

    /// <summary>
    /// Число элементов на странице
    /// </summary>
    [Range(1, 100)]
    [Required]
    public int ItemsPerPage { get; init; }
}