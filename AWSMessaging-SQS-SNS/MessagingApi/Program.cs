using MessagingApi;
using MessagingApi.Contracts;
using MessagingApi.Endpoints;
using MessagingApi.Handlers;

var builder = WebApplication.CreateBuilder(args);

var awsResources = new AWSResources();

builder.Configuration.GetSection(AWSResources.SectionName).Bind(awsResources);

builder.Services.AddAWSMessageBus(bus=>
{
    // Helps to publish the message to the SQS using AWS Messaging
    // Note: Comment the below registration when using SNS publish endpoint orders-publish-topic
    bus.AddSQSPublisher<OrderCreatedEvent>(awsResources.SQSOrderQueueUrl);

    // Helps to publish to SQS therough the SNS topic
    //bus.AddSNSPublisher<OrderCreatedEvent>(awsResources.SNSOrderTopicUrl);

    // To Consume the message which was pubished to SQS
    bus.AddSQSPoller(awsResources.SQSOrderQueueUrl);

    // Register the Handler to process the message
    bus.AddMessageHandler<OrderCreatedEventHandler, OrderCreatedEvent>();

    // BackoffPolicy on how the retry needs to happen - Resiliency handling
    // It will do the exponential retry time, until it reaches the backoff gap, which is 1 min
    bus.ConfigureBackoffPolicy(cfg=> cfg.UseCappedExponentialBackoff());

});
var app = builder.Build();


app.UseHttpsRedirection();

app.MapOrdersEndpoints();

app.Run();

