using System;
using System.Text.Json;
using MassTransit;
using Order.Contracts.Models;

namespace Shipping.Consumer;

public class ShippingConsumer : IConsumer<PublishOrder>
{
    private readonly ILogger<ShippingConsumer> _logger;
    public ShippingConsumer(ILogger<ShippingConsumer> logger)
    {
        _logger = logger;
    }
    public async Task Consume(ConsumeContext<PublishOrder> context)
    {
        try
        {
            var message = context.Message;            
            _logger.LogInformation($"Billing consumer: {context.Message.OrderId}");
            // Create JSON options for pretty printing
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            // Serialize the full message to JSON
            var jsonMessage = JsonSerializer.Serialize(message, jsonOptions);
            
            // Log the formatted JSON message
            _logger.LogInformation(
                "Received message: {JsonMessage}", 
                jsonMessage
            );
            await Console.Out.WriteLineAsync($"Billing consumer: {context.Message.OrderId}");

            _logger.LogInformation($"Message processed successfully in Shipping Consumer: {context.Message.OrderId}");
        }
        catch(Exception ex){
            _logger.LogError(ex.Message);
            throw;
        }
    }   
}
