using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Extensions.NETCore.Setup;
using BatchOperations.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAWSService<IAmazonDynamoDB>(new AWSOptions {
    Region = Amazon.RegionEndpoint.USEast1
});

builder.Services.AddScoped<IDynamoDBContext, DynamoDBContext>();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwaggerUI(options =>
    options.SwaggerEndpoint("/openApi/v1.json", "OpenApi V1"));

app.UseHttpsRedirection();

app.MapDynamoDbBatchWriteEndpoints();

app.MapDynamoDbBatchReadEndpoints();

app.MapDynamoDbBatchDeleteEndpoints();

app.Run();


