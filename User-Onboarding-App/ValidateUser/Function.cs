using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace ValidateUser;

public class Function
{
    
    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
   

    public object FunctionHandler(UserInput input, ILambdaContext context)
    {
        if (string.IsNullOrEmpty(input.Email) || 
            string.IsNullOrEmpty(input.Name) || 
            string.IsNullOrEmpty(input.Role))
        {
            return new
            {
                StatusCode = 400,
                Message = "Name, Email & Role cannot be empty."
            };
        }

        return new
        {
            input.Name,
            input.Email,
            input.Role,
            StatusCode = 200
        };
    }
}

public class UserInput
{
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
