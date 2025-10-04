using OpenTelemetry.Resources;
using SharedLib;
using ShippingProcessor;
using ShippingProcessor.Handlers;
using ShippingService;

var builder = Host.CreateApplicationBuilder(args);

var awsResources = new AWSResources();

builder.Configuration.Bind(AWSResources.SectionName, awsResources);

builder.Services.AddAWSMessageBus(bus =>
{
    bus.AddSQSPoller(awsResources.OrdersShippingQueue?.QueueUrl ?? string.Empty);

    bus.AddMessageHandler<OrderCreatedEventHandler, OrderCreatedEvent>();
});

builder.AddServiceDefaults();

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(DiagnosticsConfig.ActivitySource.Name))
    .WithTracing(tracing =>
    tracing.AddSource(DiagnosticsConfig.ActivitySource.Name));

// builder.Services.AddHostedService<Worker>();

var host = builder.Build();

host.Run();
