using System.ComponentModel.DataAnnotations;

namespace Ozon.Route256.Five.OrderService.Rest.Dto;

public sealed class OrderDto
{
    /// <summary>
    /// ИД заказа
    /// </summary>
    [Required]
    public long Id { get; init; }

    /// <summary>
    /// Количество товаров в заказе
    /// </summary>
    [Required]
    public int GoodsCount { get; init; }

    /// <summary>
    /// Общая сумма заказа
    /// </summary>
    [Required]
    public decimal TotalPrice { get; init; }

    /// <summary>
    /// Общий вес заказа в кг
    /// </summary>
    [Required]
    public decimal TotalWeight { get; init; }

    /// <summary>
    /// Тип заказа
    /// </summary>
    [Required]
    public OrderSourceDto Source { get; init; }

    /// <summary>
    /// Дата заказа
    /// </summary>
    [Required]
    public DateTimeOffset DateCreated { get; init; }

    /// <summary>
    /// Статус заказа
    /// </summary>
    [Required]
    public OrderStateDto State { get; init; }

    /// <summary>
    /// Клиент
    /// </summary>
    public CustomerDto? Customer { get; init; }

    /// <summary>
    /// Адрес доставки
    /// </summary>
    [Required]
    public AddressDto? DeliveryAddress { get; init; }

    /// <summary>
    /// Телефон
    /// </summary>
    [Required]
    public string? Phone { get; init; }
}