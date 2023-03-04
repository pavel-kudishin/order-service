using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Ozon.Route256.Five.OrderService.Core.BusinessObjects;
using Ozon.Route256.Five.OrderService.Core.Handlers.AggregateOrders;
using Ozon.Route256.Five.OrderService.Core.Handlers.ResultTypes;
using ValidationResult = FluentValidation.Results.ValidationResult;
using Ozon.Route256.Five.OrderService.Rest.Dto;
using Ozon.Route256.Five.OrderService.Rest.Extensions;

namespace Ozon.Route256.Five.OrderService.Rest.Controllers
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

            IOrderAggregationHandler handler = _serviceProvider.GetRequiredService<IOrderAggregationHandler>();
            IOrderAggregationHandler.Request handlerRequest = request.ToOrderAggregationHandlerRequestBo();
            HandlerResult<AggregatedOrdersResponseBo[]> result = await handler.Handle(handlerRequest, HttpContext.RequestAborted);

            if (result.Success == false)
            {
                return BadRequest(result.Error.BusinessError);
            }

            return result.Value.ToAggregatedOrdersResponseDto() ?? Array.Empty<AggregatedOrdersResponseDto>();
        }
    }
}
