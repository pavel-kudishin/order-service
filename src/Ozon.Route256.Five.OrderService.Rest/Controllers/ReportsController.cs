using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Ozon.Route256.Five.OrderService.Core.Handlers.AggregateOrders;
using ValidationResult = FluentValidation.Results.ValidationResult;
using Ozon.Route256.Five.OrderService.Rest.Dto;
using Ozon.Route256.Five.OrderService.Rest.Extensions;
using Ozon.Route256.Five.OrderService.Core.Handlers;
using Ozon.Route256.Five.OrderService.Domain.BusinessObjects;

namespace Ozon.Route256.Five.OrderService.Rest.Controllers
{
    /// <summary>
    /// Ручки для REST API
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public sealed class ReportsController : ControllerBase
    {
        /// <summary>
        /// 2.6 Ручка агрегации заказов по региону
        /// </summary>
        /// <param name="request"></param>
        /// <param name="validator"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<AggregatedOrdersResponseDto>>> GetAggregatedOrders(
            [FromBody][Required] AggregatedOrdersRequestDto request,
            [FromServices] IValidator<AggregatedOrdersRequestDto> validator,
            [FromServices] IOrderAggregationHandler handler)
        {
            ValidationResult validationResult =
                await validator.ValidateAsync(request, HttpContext.RequestAborted);

            if (validationResult.IsValid == false)
            {
                return BadRequest(validationResult.ToString());
            }

            IOrderAggregationHandler.Request handlerRequest = request.ToOrderAggregationHandlerRequestBo();
            HandlerResult<AggregatedOrdersBo[]> result = await handler.Handle(handlerRequest, HttpContext.RequestAborted);

            if (result.Success == false)
            {
                return BadRequest(result.Error.BusinessError);
            }

            return result.Value.ToAggregatedOrdersResponseDto() ?? Array.Empty<AggregatedOrdersResponseDto>();
        }
    }
}
