using System.ComponentModel.DataAnnotations;

namespace Ozon.Route256.Five.OrderService.Rest.Dto;

public sealed class OrdersRequestDto
{
    /// <summary>
    /// ИД регионов для фильтрации
    /// </summary>
    public int[]? RegionIds { get; set; }

    /// <summary>
    /// Фильтрация по типу заказа
    /// </summary>
    public OrderTypesDto[]? OrderTypes { get; set; }

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

    /// <summary>
    /// Направление сортировки
    /// </summary>
    [Required]
    public OrderingDirectionDto Direction { get; set; }
}