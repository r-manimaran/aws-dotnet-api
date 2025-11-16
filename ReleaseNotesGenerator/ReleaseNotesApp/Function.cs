using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.StepFunctions;
using ReleaseNotesApp.Models;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ReleaseNotesApp;

public class Function
{
    private readonly IAmazonStepFunctions _stepFunctionsClient;

    private const string StateMachineArn = "";

    public Function()
    {
        _stepFunctionsClient = new AmazonStepFunctionsClient();   
    }


    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task<APIGatewayProxyResponse> FunctionHandler(
        APIGatewayProxyRequest request, 
        ILambdaContext context)
    {
        try
        {
            if (string.IsNullOrEmpty(request.Body))
            {
                return BadRequest("Request body is required.");
            }

            var input = JsonSerializer.Deserialize<TriggerRequest>(request.Body);

            if(input == null || string.IsNullOrEmpty(input.ReleaseUrl) || 
               string.IsNullOrEmpty(input.TagName) || 
               input.TargetEmails == null || 
               input.TargetEmails.Count == 0 ||
               string.IsNullOrEmpty(input.SnsArn))
            {
                return BadRequest("Invalid input. Please provide ReleaseUrl, TagName, TargetEmails, and SnsArn.");
            }

            // Start the Step Function execution
            var startExecutionRequest = JsonSerializer.Serialize(new
            {
                releaseUrl = input.ReleaseUrl,
                tagName = input.TagName,
                targetEmails = input.TargetEmails,
                snsArn = input.SnsArn
            });
            /*
            var startExec = await _stepFunctionsClient.StartExecutionAsync(new Amazon.StepFunctions.Model.StartExecutionRequest
            {
                StateMachineArn = StateMachineArn,
                Input = startExecutionRequest
            }); */

            return new APIGatewayProxyResponse
            {
                StatusCode = 200,
                Body = JsonSerializer.Serialize(new
                {
                    message = "Step Function execution started successfully",
                    //executionArn = startExec.ExecutionArn,
                    //startDate = startExec.StartDate
                }),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
        catch (Exception ex)
        {
            context.Logger.LogError($"Error occurred: {ex.Message}");
            return new APIGatewayProxyResponse
            {
                StatusCode = 500,
                Body = JsonSerializer.Serialize(new
                {
                    message = "Internal server error",
                    error = ex.Message
                }),
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
        }
    }

    private APIGatewayProxyResponse BadRequest(string message)
    {
        return new APIGatewayProxyResponse
        {
            StatusCode = 400,
            Body = message
        };    
    }
}
