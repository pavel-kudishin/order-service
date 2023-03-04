using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Ozon.Route256.Five.OrderService.Core.BusinessObjects;
using Ozon.Route256.Five.OrderService.Core.Handlers;
using Ozon.Route256.Five.OrderService.Core.Handlers.OrderCancel;
using Ozon.Route256.Five.OrderService.Core.Handlers.OrdersByCustomerGet;
using Ozon.Route256.Five.OrderService.Core.Handlers.OrdersGet;
using Ozon.Route256.Five.OrderService.Core.Handlers.OrderStatusGet;
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
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> CancelOrder(
            [FromQuery][Required] long orderId)
        {
            IOrderCancellationHandler handler = _serviceProvider.GetRequiredService<IOrderCancellationHandler>();
            IOrderCancellationHandler.Request handlerRequest = new(orderId);
            HandlerResult result = await handler.Handle(handlerRequest, HttpContext.RequestAborted);

            if (result.Success == false)
            {
                if (result.Error is OrderNotFoundException)
                {
                    return NotFound(result.Error.BusinessError);
                }
                if (result.Error is OrderCancellationException)
                {
                    return BadRequest(result.Error.BusinessError);
                }
                return BadRequest(result.Error.BusinessError);
            }

            return Ok();
        }

        /// <summary>
        /// 2.2 Ручка возврата статуса заказа
        /// </summary>
        /// <param name="orderId">Номер заказа</param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderStatusResponseDto>> GetOrderStatus(
            [FromQuery][Required] long orderId)
        {
            IOrderStatusGettingHandler handler = _serviceProvider.GetRequiredService<IOrderStatusGettingHandler>();
            IOrderStatusGettingHandler.Request handlerRequest = new(orderId);
            HandlerResult<string> result = await handler.Handle(handlerRequest, HttpContext.RequestAborted);

            if (result.Success == false)
            {
                if (result.Error is OrderNotFoundException)
                {
                    return NotFound(result.Error.BusinessError);
                }
                return BadRequest(result.Error.BusinessError);
            }

            return Ok(new OrderStatusResponseDto()
            {
                StatusName = result.Value,
            });
        }

        /// <summary>
        /// 2.5 Ручка возврата списка заказов
        /// </summary>
        /// <param name="request"></param>
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

            IOrdersGettingHandler handler = _serviceProvider.GetRequiredService<IOrdersGettingHandler>();
            IOrdersGettingHandler.Request handlerRequest = request.ToOrdersGettingHandlerRequest();
            HandlerResult<OrderBo[]> result = await handler.Handle(handlerRequest, HttpContext.RequestAborted);

            if (result.Success == false)
            {
                return BadRequest(result.Error.BusinessError);
            }

            return result.Value.ToOrdersDto() ?? Array.Empty<OrderDto>();
        }

        /// <summary>
        /// 2.7 Ручка получения всех заказов клиента
        /// </summary>
        /// <param name="request"></param>
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

            IOrdersByCustomerGettingHandler handler = _serviceProvider.GetRequiredService<IOrdersByCustomerGettingHandler>();
            IOrdersByCustomerGettingHandler.Request handlerRequest = request.ToOrdersByCustomerGettingHandlerRequest();
            HandlerResult<OrderBo[]> result = await handler.Handle(handlerRequest, HttpContext.RequestAborted);

            if (result.Success == false)
            {
                if (result.Error is CustomerNotFoundException)
                {
                    return NotFound(result.Error.BusinessError);
                }
                return BadRequest(result.Error.BusinessError);
            }

            return result.Value.ToOrdersDto() ?? Array.Empty<OrderDto>();
        }
    }
}
