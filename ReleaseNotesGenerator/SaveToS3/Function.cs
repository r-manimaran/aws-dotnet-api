using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Model;
using Sharedlib;
using SharedLib;
using System.Text.Json;

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
    public async Task<S3Output> FunctionHandler(ReleaseNotesOutput input, ILambdaContext context)
    {
        string key = $"{input.TagName}/release-notes.md";

        var markdown = $@"
        # Release Notes {input.TagName}

        ## Executive Summary
        {input.ExecutiveSummary}

        ## Technical Details
        {input.TechnicalDetails}

        ---

        ## Raw AI Response
        {input.RawAiResponse}
       ";

        var putRequest = new PutObjectRequest
        {
            BucketName = BucketName,
            Key = $"{key}.md",
            ContentBody = markdown,
            ContentType = "text/markdown",
        };

        await _s3.PutObjectAsync(putRequest);

        string S3Location = $"s3://{BucketName}/{key}";

        return new S3Output
        { 
            TagName = input.TagName,
            S3Location = S3Location,
            Payload = input 
        };
    }
}
