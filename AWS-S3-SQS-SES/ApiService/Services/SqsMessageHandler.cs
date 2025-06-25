
using Amazon.SQS;
using Amazon.SQS.Model;
using ApiService.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace ApiService.Services;

public class SqsMessageHandler : BackgroundService
{
    private readonly IAmazonSQS _sqsClient;
    private readonly IMessageHandlerFactory _handlerFactory;
    private readonly SqsSettings _settings;
    private readonly ILogger<SqsMessageHandler> _logger;
    private readonly IServiceProvider _serviceProvider;

    public SqsMessageHandler(
        IAmazonSQS sqsClient,
        IMessageHandlerFactory handlerFactory,
        IOptions<SqsSettings> settings,
        ILogger<SqsMessageHandler> logger,
        IServiceProvider serviceProvider)
            
    {
        _sqsClient = sqsClient;
        _handlerFactory = handlerFactory;
        _settings = settings.Value;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// POLL the message from the SQS QUEUE
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var receiveRequest = new ReceiveMessageRequest
                {
                    QueueUrl = _settings.QueueUrl,
                    MaxNumberOfMessages = 10,
                    WaitTimeSeconds = 20
                };

                var response = await _sqsClient.ReceiveMessageAsync(receiveRequest, stoppingToken);

                foreach (var message in response.Messages ?? [])
                {
                    try
                    {
                        await ProcessMessageAsync(message, stoppingToken);
                    }
                    catch (Exception ex)
                    {

                        _logger.LogError(ex, "Error in processing the message:{MessageId}", message.MessageId);
                    }
                }
            }
            catch (Exception ex)
            {

                _logger.LogError(ex,"Error in SQS message handler");
                await Task.Delay(5000, stoppingToken);
            }
        }
    }

    private async Task ProcessMessageAsync(Message message, CancellationToken stoppingToken)
    {
        var messageWrapper = JsonSerializer.Deserialize<MessageWrapper>(message.Body);

        if (messageWrapper == null)
        {
            _logger.LogWarning("Failed to deserialize message wrapper.: {MessageId}", message.MessageId);
            return;
        }

        _logger.LogInformation(
            "Processing message:{MessageId}, EventType:{EventType}, correlationId:{CorrelationId}",
            message.MessageId,
            messageWrapper.EventType,
            messageWrapper.CorrelationId);

        // Route to Appropriate handled based on event Type
        switch (messageWrapper.EventType)
        {
            case nameof(DocumentUploadedEvent):
                await HandleDocumentUploadedEventAsync(messageWrapper, stoppingToken);
                break;
            default:
                _logger.LogWarning("Unkown event type:{EventType}",messageWrapper.EventType);
                break;
        }

        // Delete the message after successful processing
        await _sqsClient.DeleteMessageAsync(_settings.QueueUrl, message.ReceiptHandle, stoppingToken);

        _logger.LogInformation("Message processed sucessfull: {MessageId}",message.MessageId);

    }

    private async Task HandleDocumentUploadedEventAsync(MessageWrapper messageWrapper, CancellationToken stoppingToken)
    {
        var @event = JsonSerializer.Deserialize<DocumentUploadedEvent>(messageWrapper.EventData);
        if (@event == null)
        {
            _logger.LogWarning("Failed to deserialize DocumentUploadedEvent");
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var handler = _handlerFactory.GetHandler<DocumentUploadedEvent>(scope.ServiceProvider);

        if(handler == null)
        {
            _logger.LogWarning("No handler found for DocumentUploadedEvent");
            return;
        }

        await handler.HandleAsync(@event, stoppingToken);
    }
}
