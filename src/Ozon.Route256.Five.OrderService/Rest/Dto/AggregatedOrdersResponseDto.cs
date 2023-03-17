using System.ComponentModel.DataAnnotations;

namespace Ozon.Route256.Five.OrderService.Rest.Dto;

public sealed class AggregatedOrdersResponseDto
{
    /// <summary>
    /// Регион
    /// </summary>
    [Required]
    public string Region { get; init; } = string.Empty;

    /// <summary>
    /// Количество заказов
    /// </summary>
    [Required]
    public int OrdersCount { get; init; }

    /// <summary>
    /// Общая сумма заказов
    /// </summary>
    [Required]
    public decimal TotalOrdersPrice { get; init; }

    /// <summary>
    /// Суммарный вес
    /// </summary>
    [Required]
    public decimal TotalWeight { get; init; }

    /// <summary>
    /// Количество клиентов, сделавших заказ в этом регионе
    /// </summary>
    [Required]
    public int CustomersCount { get; init; }
}