using Microsoft.AspNetCore.Mvc;
using Ozon.Route256.Five.OrderService.Dto;
using System.ComponentModel.DataAnnotations;

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
	}
}
