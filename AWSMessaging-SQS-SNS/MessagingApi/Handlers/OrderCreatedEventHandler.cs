using AWS.Messaging;
using MessagingApi.Contracts;

namespace MessagingApi.Handlers;

public class OrderCreatedEventHandler : IMessageHandler<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedEventHandler> _logger;

    public OrderCreatedEventHandler(ILogger<OrderCreatedEventHandler> logger)
    {
        _logger = logger;
    }
    public async Task<MessageProcessStatus> HandleAsync(MessageEnvelope<OrderCreatedEvent> messageEnvelope, CancellationToken token = default)
    {
        var orderMessage = messageEnvelope.Message;
        _logger.LogInformation("Processing order {OrderId} for customer {CustomerId}", orderMessage.OrderId, orderMessage.CustomerId);

        try
        {
            await Task.Delay(1000, token); // Added Just to simulate the processing 
            _logger.LogInformation("Order:{OrderId} processed successfully", orderMessage.OrderId);
            return MessageProcessStatus.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process order {OrderId}",orderMessage.OrderId);
            return MessageProcessStatus.Failed();
        }
    }
}
