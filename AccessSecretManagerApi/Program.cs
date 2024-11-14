using AccessSecretManagerApi;
using Amazon.Runtime;
using Amazon.SecretsManager;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// call from Extensions method
builder.Services.AddAmazonSecretsManager();
builder.Services.AddAmazonS3();

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<SecretsManagerService>();
builder.Services.AddTransient<S3Service>();

//bind IOptions AWSServices model
builder.Services.Configure<AwsSettings>(builder.Configuration.GetSection("AwsSettings"));

// Adding Telemetry 
/* 
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddConsoleExporter()
            .AddOtlpExporter();
    });
builder.Services.AddOpenTelemetry()
    .WithMetrics(metricProviderBuilder =>
    {
        metricProviderBuilder
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddConsoleExporter()
            .AddOtlpExporter();

    }); */

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
