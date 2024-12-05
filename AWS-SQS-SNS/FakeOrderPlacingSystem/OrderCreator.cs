using System;
using Order.Contracts.Models;

namespace FakeOrderPlacingSystem;

public class OrderCreator : BackgroundService
{
    private readonly IOrderCreator _orderCreator;

    public OrderCreator(IOrderCreator orderCreator)
    {
        _orderCreator = orderCreator;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var newOrder = new OrderRequest($"CUST-{Random.Shared.NextInt64()}",
                new List<OrderItemRequest>()
                {
                    new OrderItemRequest($"PROD-{Random.Shared.NextInt64()}",
                          Random.Shared.Next(1, 10),
                          Random.Shared.Next(1, 1000))
                    
                });
            // call using Refit
            await _orderCreator.CreateOrder(newOrder);
             //Delay between each call by 10 seconds
            await Task.Delay(10000, stoppingToken);                
        };           
        
    }
}

