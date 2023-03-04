using System.ComponentModel.DataAnnotations;

namespace Ozon.Route256.Five.OrderService.Rest.Dto;

public sealed class AggregatedOrdersResponseDto
{
    /// <summary>
    /// Регион
    /// </summary>
    [Required]
    public RegionDto? Region { get; set; }

    /// <summary>
    /// Количество заказов
    /// </summary>
    [Required]
    public int OrdersCount { get; set; }

    /// <summary>
    /// Общая сумма заказов
    /// </summary>
    [Required]
    public decimal TotalOrdersPrice { get; set; }

    /// <summary>
    /// Суммарный вес
    /// </summary>
    [Required]
    public double TotalWeight { get; set; }

    /// <summary>
    /// Количество клиентов, сделавших заказ в этом регионе
    /// </summary>
    [Required]
    public int CustomersCount { get; set; }
}