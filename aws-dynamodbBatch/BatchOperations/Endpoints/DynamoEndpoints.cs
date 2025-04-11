using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using BatchOperations.Dtos;
using BatchOperations.Models;
using Microsoft.AspNetCore.Mvc;

namespace BatchOperations.Endpoints;

public static class DynamoEndpoints
{
    public static void MapDynamoDbBatchWriteEndpoints(this IEndpointRouteBuilder route)
    {
        var app = route.MapGroup("/api").WithOpenApi().WithTags("DynamoDB Batch Write");

        // Batch Write to Single Table in DynamoDB
        app.MapPost("/products/batch-write", async (List<CreateProductDto> productDtos, IDynamoDBContext context) =>
        {
            var products = new List<Product>();
            foreach(var product in productDtos)
            {
                products.Add(new Product(product.Name, product.Description, product.Price));
            }

            var batchWriteRequest = context.CreateBatchWrite<Product>();
            batchWriteRequest.AddPutItems(products);
            await batchWriteRequest.ExecuteAsync();
        });

        // Batch Write to Multiple Table in DynamoDB
        app.MapPost("/products/batch-write-with-audits", async (List<CreateProductDto> productsDtos, IDynamoDBContext context) =>
        {
            // Products table Batch
            var products = new List<Product>();
            foreach (var product in productsDtos)
            {
                products.Add(new Product(product.Name, product.Description, product.Price));
            }

            var batchProductWriteRequest = context.CreateBatchWrite<Product>();
            batchProductWriteRequest.AddPutItems(products);

            // Audits table Batch
            var audits = new List<Audit>();
            foreach(var product in products)
            {
                audits.Add(new Audit(product.Id, "create"));
            }

            var batchAuditWriteRequest = context.CreateBatchWrite<Audit>();
            batchAuditWriteRequest.AddPutItems(audits);

            // Now Combine both the Requests and execute
            var batchWrites = batchProductWriteRequest.Combine(batchAuditWriteRequest);
            await batchWrites.ExecuteAsync();
        });


        // Fail safe batch write
        app.MapPost("/products/fail-safe-batch-write", async (List<CreateProductDto> productDtos, 
                                                                     IAmazonDynamoDB context) =>
        {
            var products = new List<Product>();
            foreach(var product in productDtos)
            {
                products.Add(new Product(product.Name, product.Description, product.Price));
            }

            var request = new BatchWriteItemRequest
            {
                RequestItems = new Dictionary<string, List<WriteRequest>>
                {
                    {
                        "products",
                        products.Select(p=> new WriteRequest(
                            new PutRequest(new Dictionary<string, AttributeValue>
                            {
                                {"id", new AttributeValue { S= p.Id.ToString()} },
                                {"name", new AttributeValue { S = p.Name } },
                                {"description", new AttributeValue { S = p.Description } },
                                {"price", new AttributeValue { S= $"{p.Price}" } },
                            })))
                        .ToList()
                    }
                }
            };

            var maxRetries = 5;
            var delay = 200; // Initial delay of 200ms

            async Task RetryBatchWriteAsync(Dictionary<string, List<WriteRequest>> unprocessedItems)
            {
                var retryCount = 0;
                while(retryCount < maxRetries && unprocessedItems.Count > 0)
                {
                    var retryRequest = new BatchWriteItemRequest
                    {
                        RequestItems = unprocessedItems
                    };

                    var retryResponse = await context.BatchWriteItemAsync(retryRequest);

                    // check if there are still unprocessed items
                    unprocessedItems = retryResponse.UnprocessedItems;

                    if(unprocessedItems.Count == 0)
                    {
                        return; // Exit if no unprocessed items remain
                    }

                    // Apply expoential backoff
                    await Task.Delay(delay);
                    delay *= 2; // Double the delay for each retry
                    retryCount++;
                }

                if(unprocessedItems.Count >0)
                {
                    throw new Exception("Max retry attempts exceed. Some items were not processed.");
                }
            }

            var response = await context.BatchWriteItemAsync(request);

            if(response !=null && response.UnprocessedItems.Count > 0)
            {
                // Retry unprocessed items with exponential backoff
                await RetryBatchWriteAsync(response.UnprocessedItems);
            }

            return Results.Ok("Batch Write operation completed");
        });
    }

    public static void MapDynamoDbBatchReadEndpoints(this IEndpointRouteBuilder route)
    {
        var app = route.MapGroup("/api").WithOpenApi().WithTags("DynamoDB Batch Read");

        app.MapPost("/products/batch-get", async (IDynamoDBContext context, [FromBody] List<Guid> productIds) =>
        {
            var batchGet = context.CreateBatchGet<Product>();
            foreach(var id in productIds)
            {
                batchGet.AddKey(id);
            }

            await batchGet.ExecuteAsync();
            return Results.Ok(batchGet.Results);
        });
    }

    public static void MapDynamoDbBatchDeleteEndpoints(this IEndpointRouteBuilder route)
    {
        var app = route.MapGroup("/api").WithOpenApi().WithTags("DynamoDB Batch Delete"); ;

        app.MapPost("/products/batch-delete", async (IDynamoDBContext context, [FromBody] List<Guid> productsIds) =>
        {
            var batchDelete = context.CreateBatchWrite<Product>();

            foreach(var id in productsIds)
            {
                batchDelete.AddDeleteKey(id);
            }

            await batchDelete.ExecuteAsync();
        });
    }
 }
