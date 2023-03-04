using System.ComponentModel.DataAnnotations;

namespace Ozon.Route256.Five.OrderService.Rest.Dto;

public sealed class OrderDto
{
    /// <summary>
    /// ИД заказа
    /// </summary>
    [Required]
    public long Id { get; set; }

    /// <summary>
    /// Количество товаров в заказе
    /// </summary>
    [Required]
    public int ArticlesCount { get; set; }

    /// <summary>
    /// Общая сумма заказа
    /// </summary>
    [Required]
    public decimal TotalPrice { get; set; }

    /// <summary>
    /// Общий вес заказа в граммах
    /// </summary>
    [Required]
    public int TotalWeight { get; set; }

    /// <summary>
    /// Тип заказа
    /// </summary>
    [Required]
    public OrderTypesDto OrderType { get; set; }

    /// <summary>
    /// Дата заказа
    /// </summary>
    [Required]
    public DateTime DateCreated { get; set; }

    /// <summary>
    /// Статус заказа
    /// </summary>
    [Required]
    public string? Status { get; set; }

    /// <summary>
    /// Клиент
    /// </summary>
    public CustomerDto? Customer { get; set; }

    /// <summary>
    /// Адрес доставки
    /// </summary>
    [Required]
    public AddressDto? DeliveryAddress { get; set; }

    /// <summary>
    /// Телефон
    /// </summary>
    [Required]
    public string? Phone { get; set; }
}