using Amazon.Lambda.Core;
using Sharedlib;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CheckReleaseExists;

public class Function
{
    private static readonly HttpClient _client = new();
    public Function()
    {
        
    }
    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task<LambdaResult> FunctionHandler(ReleaseWorkflowInput input, ILambdaContext context)
    {
        //var apiUrl = $"https://api.github.com/repos/<ORG>/<REPO>/releases/tags/{input.TagName}";
        var apiUrl = "https://github.com/r-manimaran/dotnet-HybridCache/releases/tag/v1.0.1";

        var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
        request.Headers.Add("User-Agent", "aws-lambda");

        var response = await _client.SendAsync(request);

        input.ReleaseExists = response.IsSuccessStatusCode;

        return new LambdaResult { Payload = input };
    }
}
