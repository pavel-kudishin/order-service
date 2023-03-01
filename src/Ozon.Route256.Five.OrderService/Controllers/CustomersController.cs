using Microsoft.AspNetCore.Mvc;
using Ozon.Route256.Five.OrderService.Dto;

namespace Ozon.Route256.Five.OrderService.Controllers
{
	/// <summary>
	/// Ручки для REST API
	/// </summary>
	[Route("api/[controller]")]
	[ApiController]
	public sealed class CustomersController : ControllerBase
	{
		private readonly ILogger<CustomersController> _logger;

		public CustomersController(ILogger<CustomersController> logger)
		{
			_logger = logger;
		}

		/// <summary>
		/// 2.3 Ручка возврата списка клиентов
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("[action]")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
		{
			return Array.Empty<CustomerDto>();
		}
	}
}
