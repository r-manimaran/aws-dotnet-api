using AWS.Messaging;
using AWS.Messaging.Publishers.EventBridge;
using AWS.Messaging.Publishers.SNS;
using AWS.Messaging.Publishers.SQS;
using MessagingApi.Contracts;
using MessagingApi.Models;

namespace MessagingApi.Endpoints;

public static class OrdersEndpoints
{
    public static void MapOrdersEndpoints(this IEndpointRouteBuilder builder)
    {
        var app = builder.MapGroup("/api").WithTags("Orders");

        app.MapPost("orders", async (OrderDto order, IMessagePublisher messagePublisher, ILogger<Program> logger) =>
        {
            // validate the order
            if (order == null || order.Items.Count ==0)
            {
                logger.LogError("Invalid Request information for creating the order");
                return Results.BadRequest("Order must contain at least one item");
            }

            logger.LogError("Publishing the event to Queue using AWS Messaging Publish");
            await messagePublisher.PublishAsync(new OrderCreatedEvent
            {
                OrderId = order.OrderId,
                CustomerId = order.CustomerId,
                Items = order.Items,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount
            });

            logger.LogInformation("Message with OrderId:{OrderId} published successfully", order.OrderId);

            return Results.Ok();

        }).WithName("CreateOrder");

        
        app.MapPost("orders-publish-queue", async (OrderDto order, ISQSPublisher sqsPublisher, ILogger<Program> logger) =>
        {
            // validate the order
            if (order == null || order.Items.Count == 0)
            {
                logger.LogError("Invalid Request information for creating the order");
                return Results.BadRequest("Order must contain at least one item");
            }

            logger.LogError("Publishing the event to Queue using ISQSPublishing SendAsync");
            await sqsPublisher.SendAsync(new OrderCreatedEvent
            {
                OrderId = order.OrderId,
                CustomerId = order.CustomerId,
                Items = order.Items,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount
            }, new SQSOptions
            {
                // Additional options wll go here
            });

            logger.LogInformation("Message with OrderId:{OrderId} published successfully", order.OrderId);

            return Results.Ok();

        }).WithName("SQSPublisher");

        // uncomment the corresponding Registration in Program.cs
        // If using SNS, comment the above endpoint related to SQS and registration related to SQS in program.cs
        // If using SQS, comment the below SNS endpoint and registration related to SNS in program.cs
        app.MapPost("orders-publish-topic", async (OrderDto order, ISNSPublisher snsPublisher, ILogger<Program> logger) =>
        {
            // validate the order
            if (order == null || order.Items.Count == 0)
            {
                logger.LogError("Invalid Request information for creating the order");
                return Results.BadRequest("Order must contain at least one item");
            }

            logger.LogError("Publishing the event to SNS using ISNSPublishing PublishAsync");
            await snsPublisher.PublishAsync(new OrderCreatedEvent
            {
                OrderId = order.OrderId,
                CustomerId = order.CustomerId,
                Items = order.Items,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount
            }, new SNSOptions
            {
                // Additional options wll go here
                
            });

            logger.LogInformation("Message with OrderId:{OrderId} published successfully", order.OrderId);

            return Results.Ok();

        }).WithName("SNSPublisher");

       app.MapPost("orders-publish-evntBridge", async (OrderDto order, IEventBridgePublisher eventBridgePublisher, ILogger<Program> logger) =>
        {
            // validate the order
            if (order == null || order.Items.Count == 0)
            {
                logger.LogError("Invalid Request information for creating the order");
                return Results.BadRequest("Order must contain at least one item");
            }

            logger.LogError("Publishing the event using IEventBridgePublisher PublishAsync");
            await eventBridgePublisher.PublishAsync(new OrderCreatedEvent
            {
                OrderId = order.OrderId,
                CustomerId = order.CustomerId,
                Items = order.Items,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount
            }, new EventBridgeOptions
            {
                // Additional options wll go here

            });

            logger.LogInformation("Message with OrderId:{OrderId} published successfully", order.OrderId);

            return Results.Ok();

        }).WithName("EventBridgePublisher");*/

    }
}
