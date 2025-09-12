using Amazon.S3Vectors;
using AmazonS3Vectors;
using AmazonS3Vectors.Endpoints;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAWSService<IAmazonS3Vectors>();

builder.Services.AddLogging();

builder.Services.Configure<AmazonS3VectorsOptions>(builder.Configuration.GetSection(AmazonS3VectorsOptions.SectionName));

var awsOptions = builder.Configuration.GetSection(AmazonS3VectorsOptions.SectionName).Get<AmazonS3VectorsOptions>();

builder.Services.AddBedrockEmbeddingGenerator(awsOptions.EmbeddingModel);

builder.Services.AddHealthChecks()
    .AddCheck<S3VectorBucketCheck>("S3VectorBucket");

builder.Services.AddTransient(sp =>
{
    return new Kernel(sp);
});

var app = builder.Build();

app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) => {
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = report.Status.ToString(),
            Checks = report.Entries.Select(x => new {
                Component = x.Key,
                Status = x.Value.Status.ToString(),
                Description = x.Value.Description

            }),
            TotalDuration = report.TotalDuration
        };
        await context.Response.WriteAsJsonAsync(response);
    }
});

app.MapS3VectorEndpoints();

app.MapGet("/", () => "Hello World!");

app.Run();
