using MassTransit;
using Shipping.Consumer;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appSettings.json", optional: true, reloadOnChange: true)
                     .AddUserSecrets<Program>()
                     .AddEnvironmentVariables(); ;

// Add MassTransit
var aws_region = builder.Configuration.GetSection("AmazonSQS:Region").Value;
var aws_accessKey = builder.Configuration.GetSection("AmazonSQS:AccessKey").Value;
var aws_secretKey = builder.Configuration.GetSection("AmazonSQS:SecretKey").Value;
var host = Host.CreateDefaultBuilder(args).ConfigureServices((context, services) =>
{
    services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.SetInMemorySagaRepositoryProvider();

    x.UsingAmazonSqs((context, cfg) =>
    {
        cfg.Host(aws_region, h =>
        {
            h.AccessKey(aws_accessKey);
            h.SecretKey(aws_secretKey);
        });

        cfg.ReceiveEndpoint("order-shipping-queue", e =>
        {
            //Enable Raw message Delivery from SNS
            e.ConfigureConsumeTopology = false;

            e.ConfigureConsumer<ShippingConsumer>(context);

            //Add logging
            e.UseMessageRetry(r =>
            {
                r.Handle<Exception>(ex =>
                {
                    Console.WriteLine(ex.Message);
                    return true;
                });
                r.Intervals(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(30));
            });

            //Set prefetch count
            e.PrefetchCount = 16;
        });
        cfg.ConfigureEndpoints(context);
    });
    //Register the Consumer
    x.AddConsumer<ShippingConsumer>();
});
    services.AddLogging(logging => {
        logging.AddConsole();
        logging.SetMinimumLevel(LogLevel.Information);
    });
}).Build();
Console.WriteLine("Shipping Consumer with MassTransit is running...");
await host.RunAsync();
