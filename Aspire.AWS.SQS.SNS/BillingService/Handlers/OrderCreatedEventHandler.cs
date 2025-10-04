using AWS.Messaging;
using SharedLib;

namespace BillingProcessor.Handlers;

public class OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger) : IMessageHandler<OrderCreatedEvent>
{
    public async Task<MessageProcessStatus> HandleAsync(MessageEnvelope<OrderCreatedEvent> messageEnvelope, CancellationToken token = default)
    {
        using(var activity = DiagnosticsConfig.ActivitySource.StartActivity("OrderCreatedEventHandler.ProcessMessage"))
        {
            var order = messageEnvelope.Message;

            activity?.SetTag("order.id", order.OrderId);
            activity?.SetTag("order.CustomerId", order.CustomerId);

            logger.LogInformation("Processing order {OrderId} for customer {CustomerId}", order.OrderId, order.CustomerId);
            try
            {
                await Task.Delay(1000,token);
                logger.LogInformation("Order {OrderId} for Customer {CustomerId} Processed successfully", order.OrderId, order.CustomerId);
                return MessageProcessStatus.Success();

            }
            catch(Exception ex)
            {
                logger.LogError(ex,"Failed to process order {OrderId}",order.OrderId);
                return MessageProcessStatus.Failed();
            }
                    
        }
    }
}
