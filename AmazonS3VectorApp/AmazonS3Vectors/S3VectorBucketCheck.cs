using Amazon.S3Vectors;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace AmazonS3Vectors;

public class S3VectorBucketCheck : IHealthCheck
{
    private readonly ILogger<S3VectorBucketCheck> _logger;
    private readonly IAmazonS3Vectors _amazonS3Vectors;
    private readonly AmazonS3VectorsOptions _amazonS3VectorsOptions;
    public S3VectorBucketCheck(IOptions<AmazonS3VectorsOptions> options, ILogger<S3VectorBucketCheck> logger, IAmazonS3Vectors amazonS3Vectors)
    {
        _logger = logger;
        _amazonS3Vectors = amazonS3Vectors;
        _amazonS3VectorsOptions = options.Value;
    }
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new Amazon.S3Vectors.Model.GetVectorBucketRequest
            {
                VectorBucketName = _amazonS3VectorsOptions.VectorBucketName
            };

            var response = await _amazonS3Vectors.GetVectorBucketAsync(request, cancellationToken);
            if (response != null && response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                return HealthCheckResult.Healthy($"S3 Vector Bucket '{_amazonS3VectorsOptions.VectorBucketName}' exists and accessible");
            }
            else
            {
                return HealthCheckResult.Unhealthy($"S3 Vector Bucket '{_amazonS3VectorsOptions.VectorBucketName}' not exists");
            }
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"S3 Vector Bucket '{_amazonS3VectorsOptions.VectorBucketName}' not exists");
        }
    }
}
