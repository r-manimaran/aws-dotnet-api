using AWS.Messaging;
using SharedLib;

namespace OrderService.Service;

public class OrdersService
{
    private readonly ILogger<OrdersService> _logger;
    private readonly IMessagePublisher _messagePublisher;
    public List<OrderDto> Orders { get; set; } = new List<OrderDto>();
    public OrdersService(ILogger<OrdersService> logger, IMessagePublisher messagePublisher)
    {
        _logger = logger;
        _messagePublisher = messagePublisher;
    }

    public async Task CreateOrder(OrderDto order)
    {
        // Save to Database
        Orders.Add(order);

        // Publish to Message Topic
        await _messagePublisher.PublishAsync(new OrderCreatedEvent
        {
            OrderId = order.Id,
            CustomerId = order.Customer.Id,
            Items = order.Items,
            TotalAmount = order.TotalPrice,
            OrderDate = order.OrderDate
        });
        _logger.LogInformation("Published the message successfully to the Topic");
    }
}
