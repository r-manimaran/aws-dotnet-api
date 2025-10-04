using AWS.Messaging;
using SharedLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingProcessor.Handlers;

public class OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger) : IMessageHandler<OrderCreatedEvent>
{
    public async Task<MessageProcessStatus> HandleAsync(MessageEnvelope<OrderCreatedEvent> messageEnvelope, CancellationToken token = default)
    {
        var orderMessage = messageEnvelope.Message;
        logger.LogInformation("Processing Order {OrderId} for customer {CustomerId}", orderMessage.OrderId, orderMessage.CustomerId);

        try
        {
            await  Task.Delay(1000, token);
            logger.LogInformation("Order: {OrderId} processed successfully", orderMessage.OrderId);
            return MessageProcessStatus.Success();
        }
        catch (Exception ex) 
        {
            logger.LogError(ex,"Failed to process order {orderId}", orderMessage.OrderId);
            return MessageProcessStatus.Failed();
        }
    }
}
