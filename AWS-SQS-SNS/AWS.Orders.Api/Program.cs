using Amazon.Auth.AccessControlPolicy.ActionIdentifiers;
using AWS.Orders.Api.Services;
using MassTransit;
using Microsoft.Extensions.Logging;
using Order.Contracts.Models;
using Scalar.AspNetCore;
using Amazon.SimpleNotificationService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();    
});

builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Add MassTransit
var aws_region = builder.Configuration.GetSection("AmazonSQS:Region").Value;
var aws_accessKey = builder.Configuration.GetSection("AmazonSQS:AccessKey").Value;
var aws_secretKey = builder.Configuration.GetSection("AmazonSQS:SecretKey").Value;


builder.Services.AddMassTransit(configure =>{    
  
    configure.UsingAmazonSqs((context, cfg) =>
    {
        cfg.Host(aws_region, h =>
        {
            h.AccessKey(aws_accessKey);
            h.SecretKey(aws_secretKey);
        });
        // configure SNS topic endpoint
        cfg.Message<PublishOrder>(t => 
        {
            t.SetEntityName("order-topic");    
        });
        cfg.ConfigureEndpoints(context);        
    });    

});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
