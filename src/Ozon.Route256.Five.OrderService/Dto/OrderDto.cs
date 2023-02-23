using System.ComponentModel.DataAnnotations;

namespace Ozon.Route256.Five.OrderService.Dto;

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
	/// Общий вес заказа
	/// </summary>
	[Required]
	public decimal TotalWeight { get; set; }

	/// <summary>
	/// Тип заказа
	/// </summary>
	[Required]
	public OrderTypes OrderType { get; set; }

	/// <summary>
	/// Дата заказа
	/// </summary>
	[Required]
	public DateTime DateCreated { get; set; }

	/// <summary>
	/// Откуда сделан заказа (регион)
	/// </summary>
	[Required]
	public RegionDto? Region { get; set; }

	/// <summary>
	/// Статус заказа
	/// </summary>
	[Required]
	public string? Status { get; set; }

	/// <summary>
	/// Имя клиента (имя и фамилия)
	/// </summary>
	[Required]
	public string? CustomerName { get; set; }

	/// <summary>
	/// Адрес доставки
	/// </summary>
	[Required]
	public AddressDto? DeliveryAddress { get; set; }

	/// <summary>
	/// Телефон
	/// </summary>
	[Required]
	public long? Phone { get; set; }
}