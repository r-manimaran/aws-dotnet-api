using Amazon.Lambda.Core;
using AWS.Lambda.Powertools.EventHandler.Resolvers;
using AWS.Lambda.Powertools.EventHandler.Resolvers.BedrockAgentFunction.Models;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace BedrockFunction;

public class Function
{
    private readonly BedrockAgentFunctionResolver _resolver;

    public Function()
    {
       _resolver = new BedrockAgentFunctionResolver();

       _resolver.Tool("getWeatherForCity", (string cityName) =>  $"The weather in {cityName} is sunny with a high of 75°F.");
        
    }

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public BedrockFunctionResponse FunctionHandler(BedrockFunctionRequest input, ILambdaContext context)
    {
        return _resolver.Resolve(input, context);
    }
}
