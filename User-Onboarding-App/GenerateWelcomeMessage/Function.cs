using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Amazon.Lambda.Core;
using Amazon.Runtime.Internal.Transform;
using System.Data;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace GenerateWelcomeMessage;

public class Function
{
    private readonly IAmazonBedrockRuntime _amazonBedrock;
    public Function()
    {
        _amazonBedrock = new AmazonBedrockRuntimeClient();
    }

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public string? FunctionHandler(Profile input, ILambdaContext context)
    {
        string modelId = Environment.GetEnvironmentVariable("BEDROCK_MODEL_ID") ?? "us.anthropic.claude-3-5-sonnet-20241022-v2:0";

        string prompt= BuildPrompt(input);

        var request = new InvokeModelRequest
        {
            ModelId = modelId,
            ContentType = "application/json",
            Accept = "application/json",
            Body = new MemoryStream(
                Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new
                {
                    anthropic_version = "bedrock-2023-05-31",
                    max_tokens = 2000,
                    messages = new[]
                {
                    new
                    {
                        role = "user",
                        content = prompt
                    }
                }
                })))
        };
        var response = _amazonBedrock.InvokeModelAsync(request);
        string responseBody = Encoding.UTF8.GetString(response.Result.Body.ToArray());
        context.Logger.LogLine($"Bedrock Response:{responseBody}");
        var json = JsonSerializer.Deserialize<JsonElement>(responseBody);
        var resultText = json.GetProperty("content")[0].GetProperty("text").GetString();

        return resultText;
    }
    private string BuildPrompt(Profile profile)
    {
        return $"You are a helpful assistant. Please provide a short welcome message for the user named {profile.Name} for the role { profile.Role} for the company ABC Technologies." +
            $"The company is located in the USA and they are a software company. The company is a global leader in the field of software development.";

    }
    
}

public class Profile
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }

}
