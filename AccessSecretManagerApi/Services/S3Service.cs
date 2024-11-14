using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;

namespace AccessSecretManagerApi;

public class S3Service
{
    private readonly IAmazonS3 _s3Client;
    private readonly IOptions<AwsSettings> _awsSettings;
    private readonly ILogger<S3Service> _logger;
    private readonly string _bucketName = "maag";

    
    public S3Service(IAmazonS3 s3Client, IOptions<AwsSettings> awsSettings,
                    ILogger<S3Service> logger)
    {
        this._s3Client = s3Client;
        this._awsSettings = awsSettings;
        this._logger = logger;
    }

    public async Task<string> UploadFile(IFormFile formFile){
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName);

        using (var stream = new MemoryStream())
        {
            formFile.CopyTo(stream);
            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = fileName,
                InputStream = stream
            };
           await _s3Client.PutObjectAsync(request);
        }

        return $"File {formFile.FileName} uploaded Sucessfully.";
    }
    /// <summary>
    /// Get the PresignedUrl
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public async Task<string> GetPresignedUrl(string fileName)
    {
        _logger.LogInformation($"Access Key: {_awsSettings.Value.AccessKey}");
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key = fileName,
            Expires = DateTime.Now.AddMinutes(10)
        };

        var url = _s3Client.GetPreSignedURL(request);

        return await Task.FromResult(url);
    }


}
