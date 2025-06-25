
using Amazon.SQS;
using ApiService.Models;
using AWS.Messaging;
using Microsoft.Extensions.Options;

namespace ApiService.Services;

public class SqsMessageHandlerAWSMessaging : IMessageHandler<DocumentUploadedEvent>
{
    private readonly IAmazonSQS _sqsClient;
    private readonly ILogger<SqsMessageHandlerAWSMessaging> _logger;
    private readonly SqsSettings _settings;

    public SqsMessageHandlerAWSMessaging(
        IAmazonSQS sqsClient,
        IOptions<SqsSettings> settings,
        ILogger<SqsMessageHandlerAWSMessaging> logger)
    {
        _sqsClient = sqsClient;
        _logger = logger;
        _settings = settings.Value;
    }

    public async Task<MessageProcessStatus> HandleAsync(MessageEnvelope<DocumentUploadedEvent> messageEnvelope, CancellationToken token = default)
    {
        try
        {
            var message = messageEnvelope.Message;

            return MessageProcessStatus.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process the message:{MessageId}", messageEnvelope.Id);
            return MessageProcessStatus.Failed();
        }
    }
}
