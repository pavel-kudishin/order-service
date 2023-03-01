using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Ozon.Route256.Five.OrderService.Dto;
using System.ComponentModel.DataAnnotations;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace Ozon.Route256.Five.OrderService.Controllers
{
	/// <summary>
	/// Ручки для REST API
	/// </summary>
	[Route("api/[controller]")]
	[ApiController]
	public sealed class OrdersController : ControllerBase
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly ILogger<OrdersController> _logger;

		public OrdersController(
			IServiceProvider serviceProvider,
			ILogger<OrdersController> logger)
		{
			_serviceProvider = serviceProvider;
			_logger = logger;
		}

		/// <summary>
		/// 2.1 Ручка отмены заказа
		/// </summary>
		/// <param name="orderId">Номер заказа</param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("[action]")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult> CancelOrder(
			[FromQuery][Required] long orderId)
		{
			if (orderId <= 0 || orderId >= 20)
			{
				return NotFound();
			}
			if (orderId < 10)
			{
				return Ok();
			}

			return BadRequest($"Заказ #{orderId} нельзя отменить");
		}

		/// <summary>
		/// 2.2 Ручка возврата статуса заказа
		/// </summary>
		/// <param name="orderId">Номер заказа</param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("[action]")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<OrderStatusResponseDto>> GetOrderStatus(
			[FromQuery][Required] long orderId)
		{
			if (orderId <= 0 || orderId >= 20)
			{
				return NotFound();
			}

			return Ok(new OrderStatusResponseDto()
			{
				StatusName = "Создан",
			});
		}

		/// <summary>
		/// 2.5 Ручка возврата списка заказов
		/// </summary>
		/// <param name="request"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("[action]")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders(
			[FromBody][Required] OrdersRequestDto request)
		{
			IValidator<OrdersRequestDto> validator =
				_serviceProvider.GetRequiredService<IValidator<OrdersRequestDto>>();
			ValidationResult validationResult =
				await validator.ValidateAsync(request, HttpContext.RequestAborted);

			if (validationResult.IsValid == false)
			{
				return BadRequest(validationResult.ToString());
			}

			return Array.Empty<OrderDto>();
		}

		/// <summary>
		/// 2.7 Ручка получения всех заказов клиента
		/// </summary>
		/// <param name="request"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("[action]")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByCustomer(
			[FromBody][Required] OrdersByCustomerRequestDto request)
		{
			IValidator<OrdersByCustomerRequestDto> validator =
				_serviceProvider.GetRequiredService<IValidator<OrdersByCustomerRequestDto>>();
			ValidationResult validationResult =
				await validator.ValidateAsync(request, HttpContext.RequestAborted);

			if (validationResult.IsValid == false)
			{
				return BadRequest(validationResult.ToString());
			}

			if (request.CustomerId <= 0 || request.CustomerId >= 20)
			{
				return NotFound();
			}

			return Array.Empty<OrderDto>();
		}
	}
}
