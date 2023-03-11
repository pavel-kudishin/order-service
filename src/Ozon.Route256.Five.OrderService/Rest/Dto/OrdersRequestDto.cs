using System.ComponentModel.DataAnnotations;

namespace Ozon.Route256.Five.OrderService.Rest.Dto;

public sealed class OrdersRequestDto
{
    /// <summary>
    /// Регионы для фильтрации
    /// </summary>
    public string[]? Regions { get; init; }

    /// <summary>
    /// Фильтрация по типу заказа
    /// </summary>
    public OrderSourceDto[]? Sources { get; init; }

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

    /// <summary>
    /// Направление сортировки
    /// </summary>
    [Required]
    public OrderingDirectionDto Direction { get; init; }
}