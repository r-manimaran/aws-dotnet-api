using Amazon.S3;
using Amazon.SimpleEmail;
using Amazon.SQS;
using ApiService.Endpoints;
using ApiService.Models;
using ApiService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());

builder.Services.AddAWSService<IAmazonS3>();

builder.Services.AddAWSService<IAmazonSimpleEmailService>();

builder.Services.AddAWSService<IAmazonSQS>();


// Configure the Settings
builder.Services.Configure<S3Settings>(builder.Configuration.GetSection(S3Settings.SectionName));

builder.Services.Configure<SESSettings>(builder.Configuration.GetSection(SESSettings.SectionName));

builder.Services.Configure<SqsSettings>(builder.Configuration.GetSection(SqsSettings.SectionName));

// Register Services
builder.Services.AddScoped<IFileStorageService, S3FileStorageService>();

builder.Services.AddScoped<IEmailSender, SesEmailSender>();

builder.Services.AddScoped<IMessagePublisher, SqsMessagePublisher>();

// Add Event Handlers
builder.Services.AddTransient<IEventHandler<DocumentUploadedEvent>, DocumentUploadedEventHandler>();

// Register Message handler Factor
builder.Services.AddTransient<IMessageHandlerFactory, MessageHandlerFactory>();

// Register background Service
builder.Services.AddHostedService<SqsMessageHandler>();

// Un-Comment the below section to use AWS Messaging Handler
// Same way we need to use SqsPublisher to publish
/*
builder.Services.AddAWSMessageBus(bus =>
{
    bus.AddSQSPublisher<DocumentUploadedEvent>(builder.Configuration.GetSection("SqsSettings:QueueUrl").Value);
   
    bus.AddMessageHandler<SqsMessageHandlerAWSMessaging, DocumentUploadedEvent>();

    bus.AddSQSPoller(builder.Configuration.GetSection("SqsSettings:QueueUrl").Value);

    bus.ConfigureBackoffPolicy(opt=> opt.UseCappedExponentialBackoff());
}); */

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapOpenApi();

app.UseHttpsRedirection();

app.MapDocumentsEndpoints();

app.Run();
