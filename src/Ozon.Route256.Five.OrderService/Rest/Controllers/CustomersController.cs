using Microsoft.AspNetCore.Mvc;
using Ozon.Route256.Five.OrderService.Core.BusinessObjects;
using Ozon.Route256.Five.OrderService.Core.Handlers.CustomersGet;
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
    public sealed class CustomersController : ControllerBase
    {
        private readonly ILogger<CustomersController> _logger;
        private readonly IServiceProvider _serviceProvider;

        public CustomersController(
            ILogger<CustomersController> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 2.3 Ручка возврата списка клиентов
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
        {
            ICustomersGettingHandler handler = _serviceProvider.GetRequiredService<ICustomersGettingHandler>();
            HandlerResult<CustomerBo[]> result = await handler.Handle(HttpContext.RequestAborted);

            return result.Value.ToCustomersDto() ?? Array.Empty<CustomerDto>();
        }
    }
}
