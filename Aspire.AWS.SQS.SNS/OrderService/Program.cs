using OrderService.Endpoints;
using OrderService.Service;
using SharedLib;

var builder = WebApplication.CreateBuilder(args);

var awsResources = new AWSResources();

builder.Configuration.Bind(AWSResources.SectionName, awsResources);

builder.Services.AddAWSMessageBus(bus =>
{
    bus.AddSNSPublisher<OrderCreatedEvent>(awsResources.OrdersTopic?.TopicArn);
});
builder.Services.AddScoped<OrdersService>();

builder.AddServiceDefaults();

var app = builder.Build();

app.MapDefaultEndpoints();

app.MapGet("/", () => "Hello World!");

app.MapOrdersEndpoint();

app.Run();
