using AWS.Orders.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.Contracts.Models;

namespace AWS.Orders.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly IOrderService _orderService;

        public OrderController(ILogger<OrderController> logger,
                               IOrderService orderService)
        {
            _logger = logger;
            _orderService = orderService;
        }

        [HttpPost]
        [EndpointSummary("Create New Order")]
        [EndpointDescription("Create new Order and Publish to SNS--SQS")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderRequest newOrder)
        {
            try
            {
                var newProcessedOrder = await _orderService.CreateOrderAndPublish(newOrder);
                return Ok(newProcessedOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return StatusCode(500, "Error creating order");
            }
        }
    }
}
