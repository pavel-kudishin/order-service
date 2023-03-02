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
	public sealed class ReportsController : ControllerBase
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly ILogger<ReportsController> _logger;

		public ReportsController(
			IServiceProvider serviceProvider, 
			ILogger<ReportsController> logger)
		{
			_serviceProvider = serviceProvider;
			_logger = logger;
		}

		/// <summary>
		/// 2.6 Ручка агрегации заказов по региону
		/// </summary>
		/// <param name="request"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		[HttpPost]
		[Route("[action]")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<IEnumerable<AggregatedOrdersResponseDto>>> GetAggregatedOrders(
			[FromBody][Required] AggregatedOrdersRequestDto request)
		{
			IValidator<AggregatedOrdersRequestDto> validator =
				_serviceProvider.GetRequiredService<IValidator<AggregatedOrdersRequestDto>>();
			ValidationResult validationResult =
				await validator.ValidateAsync(request, HttpContext.RequestAborted);

			if (validationResult.IsValid == false)
			{
				return BadRequest(validationResult.ToString());
			}

			return BadRequest();
		}
	}
}
