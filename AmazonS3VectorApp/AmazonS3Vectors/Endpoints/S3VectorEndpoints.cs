using Amazon.Runtime.Documents;
using Amazon.S3Vectors;
using Amazon.S3Vectors.Model;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;

namespace AmazonS3Vectors.Endpoints;

public static class S3VectorEndpoints
{
    public static void MapS3VectorEndpoints(this IEndpointRouteBuilder route)
    {
        var app = route.MapGroup("/api/s3vectors")
                       .WithTags("S3Vectors")
                       .WithOpenApi();

        app.MapGet("/health", () => Results.Ok("Service is healthy"))
           .WithName("HealthCheck")
           .WithTags("S3Vectors");

        app.MapPost("/upload", async (IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator, 
                                             IAmazonS3Vectors amazonS3Vector, IOptions<AmazonS3VectorsOptions> options,
                                             ILogger<Program> logger) =>
        {
           List<(string Title, string Content, float[] Embeddings)> embeddings = [];
           
            logger.LogInformation("Uploading documents to S3 Vector Index {IndexName} in the S3 Bucket {BucketName}",

                options.Value.VectorIndexName, options.Value.VectorBucketName);
            // Read the JSON file containing the articles
            string filePath = "Data/HelpfulTips.json";
            logger.LogInformation("Reading articles from {FilePath}", filePath);
            string jsonContent = await File.ReadAllTextAsync(filePath);

            var articles = System.Text.Json.JsonSerializer.Deserialize<List<Article>>(jsonContent);
            logger.LogInformation("Read {ArticleCount} articles from the JSON file", articles?.Count ?? 0);

            foreach (var article in articles)
            {
                var embedding = await embeddingGenerator.GenerateAsync(article.Content);

                embeddings.Add((article.Title, article.Content, embedding.Vector.ToArray()));                
            }
                
           logger.LogInformation("Generated embeddings for {EmbeddingCount} articles", embeddings.Count);
            var request = new PutVectorsRequest
            {
                VectorBucketName = options.Value.VectorBucketName,
                IndexName = options.Value.VectorIndexName,
                Vectors = embeddings.Select(e => new PutInputVector
                {
                    Key = $"article-{Guid.NewGuid()}",
                    Data = new VectorData
                    {
                        Float32 = [.. e.Embeddings]
                    },
                    Metadata = new Document(new Dictionary<string, Document>
                    {
                        { "source", e.Title},
                        {"content", new Document(e.Content) },
                        { "timestamp", DateTime.UtcNow.ToString("o") }
                    })

                }).ToList()
            };
            logger.LogInformation("Uploading {VectorCount} vectors to the S3 Vector Index", request.Vectors.Count);
            var response = await amazonS3Vector.PutVectorsAsync(request);
            logger.LogInformation("Uploaded vectors to the S3 Vector Index: {Status}", response.HttpStatusCode);

            return Results.Ok(response);
        })
        .WithName("UploadDocuments")
        .WithTags("S3Vectors");

        app.MapGet("/search", async (string searchTerm, IEmbeddingGenerator<string, Embedding<float>> embedding,
                                             IAmazonS3Vectors amazonS3Vector,
                                             IOptions<AmazonS3VectorsOptions> options,
                                             ILogger<Program> logger) =>
        {
            logger.LogInformation("Searching for: {SearchTerm}", searchTerm);
            var searchEmbedding = await embedding.GenerateAsync(searchTerm);
            logger.LogInformation("Generated embedding for the search term");

            var queryRequest = new QueryVectorsRequest
            {
                VectorBucketName = options.Value.VectorBucketName,
                IndexName = options.Value.VectorIndexName,
                ReturnDistance = true,
                ReturnMetadata = true,
                TopK = 3,
                //Filter =
                QueryVector = new VectorData
                {
                    Float32 = [.. searchEmbedding.Vector.ToArray()]
                }
            };
            logger.LogInformation("Querying the S3 Vector Index {IndexName} in the S3 Bucket {BucketName}", 
                options.Value.VectorIndexName, options.Value.VectorBucketName);

            var queryResponse = await amazonS3Vector.QueryVectorsAsync(queryRequest);

            foreach (var outputVector in queryResponse.Vectors)
            {
                logger.LogInformation("Key: {Key}, Distance: {Distance}, Source: {Source}",
                    outputVector.Key,
                    outputVector.Distance,
                    outputVector.Metadata.AsDictionary()["source"]);
            }
            logger.LogInformation("Query completed with {ResultCount} results", queryResponse.Vectors.Count);
            return Results.Ok(new {
                Results = queryResponse.Vectors.Select(outputVector => new ResponseData
                {
                    Key = outputVector.Key,
                    Distance = outputVector.Distance,
                    Title = outputVector.Metadata.AsDictionary()["source"].ToString(),
                    Content = outputVector.Metadata.AsDictionary()["content"].ToString()
                })
            });
        }).WithName("QueryDocuments")
        .WithTags("S3Vectors");

        app.MapDelete("/deleteVectorIndex/{bucketName}/{indexName}", async (string bucketName, string indexName, 
            IAmazonS3Vectors amazonS3Vector,
             ILogger<Program> logger) =>
        {
            var deleteRequest = new DeleteIndexRequest
            {
                IndexName = indexName,
                VectorBucketName = bucketName
            };

            var response = await amazonS3Vector.DeleteIndexAsync(deleteRequest);
            logger.LogInformation("Deleted the Index {IndexName} in the S3 Bucket {BucketName}", indexName, bucketName);

            return Results.Ok(response);

        }).WithName("DeleteVectorIndex")
        .WithTags("S3Vectors");
    }
}
