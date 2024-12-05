using System;

using Bogus;
using MassTransit;
using Order.Contracts.Models;

namespace AWS.Orders.Api.Services;

public class OrderService : IOrderService
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OrderService> _logger;

    public OrderService(IPublishEndpoint publishEndpoint, 
                        IConfiguration configuration,
                        ILogger<OrderService> logger)
    {
        _publishEndpoint = publishEndpoint;
        _configuration = configuration;
        _logger = logger;
    }
    public async Task<PublishOrder> CreateOrderAndPublish(OrderRequest newOrder)
    {
        //For testing log the keys
        _logger.LogInformation(_configuration.GetSection("AmazonSQS:AccessKey").Value);
        var faker = new Faker();
        var publishOrder = new PublishOrder
                        {
                        OrderId =Guid.NewGuid(),
                        CustomerId = newOrder.CustomerId,
                        CustomerName = faker.Name.FullName(),
                        CustomerEmail = faker.Internet.Email(),          
                        OrderDate = DateTime.UtcNow,
                        Items = newOrder.Items,
                        TotalAmount = newOrder.Items.Sum(x => x.Price * x.Quantity)
                        };
        // publish using MassTransit to SNS topic
        _logger.LogInformation("Publishing order to SNS topic");
        await _publishEndpoint.Publish(publishOrder);

        return publishOrder;
    }
}
