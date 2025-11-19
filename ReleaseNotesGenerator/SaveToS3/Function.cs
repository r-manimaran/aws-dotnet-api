using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;
using Sharedlib;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SaveToS3;

public class Function
{
    private readonly IAmazonS3 _s3 = new AmazonS3Client();

    private const string BucketName = "release-notes-maran";
    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task<LambdaResult> FunctionHandler(ReleaseWorkflowInput input, ILambdaContext context)
    {
        string key = $"{input.TagName}/release-notes.txt";

        var putRequest = new PutObjectRequest
        {
            BucketName = BucketName,
            Key = key,
            ContentBody = input.ReleaseNotes,
        };

        await _s3.PutObjectAsync(putRequest);

        input.S3Location = $"s3://{BucketName}/{key}";

        return new LambdaResult { Payload = input };
    }
}
