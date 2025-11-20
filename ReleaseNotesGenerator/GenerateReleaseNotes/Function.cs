using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Amazon.Lambda.Core;
using SharedLib;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace GenerateReleaseNotes;

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
    public async Task<ReleaseNotesOutput> FunctionHandler(ReleaseNotesInput input, ILambdaContext context)
    {

        // 1. Define the Model
       // string modelId = Environment.GetEnvironmentVariable("BEDROCK_MODEL_ID") ?? "anthropic.claude-3-5-sonnet-20241022-v2:0";
        string modelId = Environment.GetEnvironmentVariable("BEDROCK_MODEL_ID") ?? "us.anthropic.claude-3-5-sonnet-20241022-v2:0";

        
        // 2. Create the Prompt
        string prompt = BuildPrompt(input);

        // 3. set the invokeRequest
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

        // 4. call the InvokeModelAsync
        var response = _amazonBedrock.InvokeModelAsync(request);
        string responseBody = Encoding.UTF8.GetString(response.Result.Body.ToArray());
        context.Logger.LogLine($"Bedrock Response: {responseBody}");
        var json = JsonSerializer.Deserialize<JsonElement>(responseBody);
        var resultText = json.GetProperty("content")[0].GetProperty("text").GetString();

        var parsed = ParseReleaseNotes(resultText);

        return new ReleaseNotesOutput
        {

            TagName = input.Tag,
            ExecutiveSummary = parsed.ExecutiveSummary,
            TechnicalDetails = parsed.TechnicalDetails,
            RawAiResponse = resultText
        };
    }

    private string BuildPrompt(ReleaseNotesInput input)
    {
        var commits = string.Join("\n", input.CommitHistory.Select(c=> 
            $"- {c.Date}: {c.Message} (Author: {c.Author}, SHA:{c.Sha})"));
        return $@"
                Generate dual-audience release notes for production tag '{input.Tag}'.

        ### Repository Tag
        {input.Tag}

        ### Primary Commit
        Message: {input.CommitMessage}
        Author: {input.CommitAuthor}
        Date: {input.CommitDate}

        ### Commit History Since {input.PreviousTag}
        {commits}
        ### Instructions
        1. Provide a clear **Executive Summary** suitable for business stakeholders.
        2. Provide detailed **Technical Details** suitable for developers and technical teams.
        3. Highlight:
            - Major new features
            - Bug fixes
            - Performance improvements and optimizations
            - Breaking changes
        4.Format the output in markdown with distinct sections for Executive Summary and Technical Details.

        Return exactly in th structure:
        Executive Summary:
        <Your executive summary here>

        Technical Details:
        <Your technical details here>
        ";
       
    }
    private (string ExecutiveSummary, string TechnicalDetails) ParseReleaseNotes(string aiResponse)
    {
        var execSummaryMarker = "Executive Summary:";
        var techDetailsMarker = "Technical Details:";
        var execStart = aiResponse.IndexOf(execSummaryMarker);
        var techStart = aiResponse.IndexOf(techDetailsMarker);
        if (execStart == -1 || techStart == -1 || techStart <= execStart)
        {
            throw new Exception("AI response format is invalid.");
        }
        var executiveSummary = aiResponse[(execStart + execSummaryMarker.Length)..techStart].Trim();
        var technicalDetails = aiResponse[(techStart + techDetailsMarker.Length)..].Trim();
        return (executiveSummary, technicalDetails);
    }
}
