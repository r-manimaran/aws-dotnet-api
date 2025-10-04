using BillingProcessor;
using BillingProcessor.Handlers;
using BillingService;
using OpenTelemetry.Resources;
using SharedLib;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

var awsResources = new AWSResources();

builder.Configuration.Bind(AWSResources.SectionName, awsResources);

builder.Services.AddAWSMessageBus(bus =>
{
    bus.AddSQSPoller(awsResources.OrdersBillingQueue?.QueueUrl ?? string.Empty);

    bus.AddMessageHandler<OrderCreatedEventHandler, OrderCreatedEvent>();
});

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource=> resource.AddService(DiagnosticsConfig.ActivitySource.Name))
    .WithTracing(tracing=> tracing.AddSource(DiagnosticsConfig.ActivitySource.Name));

//builder.Services.AddHostedService<Worker>();

var host = builder.Build();

host.Run();
