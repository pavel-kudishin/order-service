using Microsoft.AspNetCore.Mvc;
using Ozon.Route256.Five.OrderService.Core.BusinessObjects;
using Ozon.Route256.Five.OrderService.Core.Handlers.RegionsGet;
using Ozon.Route256.Five.OrderService.Core.Handlers.ResultTypes;
using Ozon.Route256.Five.OrderService.Rest.Dto;
using Ozon.Route256.Five.OrderService.Rest.Extensions;

namespace Ozon.Route256.Five.OrderService.Rest.Controllers
{
    /// <summary>
    /// Ручки для REST API
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public sealed class RegionsController : ControllerBase
    {
        /// <summary>
        /// 2.4 Ручка возврата списка регионов
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RegionDto>>> GetRegions(
            [FromServices] IRegionsGettingHandler handler)
        {
            HandlerResult<RegionBo[]> result = await handler.Handle(HttpContext.RequestAborted);

            return result.Value.ToRegionsDto() ?? Array.Empty<RegionDto>();
        }
    }
}
