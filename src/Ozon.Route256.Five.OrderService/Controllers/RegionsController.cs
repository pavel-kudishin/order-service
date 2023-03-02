using Microsoft.AspNetCore.Mvc;
using Ozon.Route256.Five.OrderService.Dto;

namespace Ozon.Route256.Five.OrderService.Controllers
{
	/// <summary>
	/// Ручки для REST API
	/// </summary>
	[Route("api/[controller]")]
	[ApiController]
	public sealed class RegionsController : ControllerBase
	{
		private readonly ILogger<RegionsController> _logger;

		public RegionsController(ILogger<RegionsController> logger)
		{
			_logger = logger;
		}

		/// <summary>
		/// 2.4 Ручка возврата списка регионов
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		[HttpGet]
		[Route("[action]")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<IEnumerable<RegionDto>>> GetRegions()
		{
			return Array.Empty<RegionDto>();
		}
	}
}
