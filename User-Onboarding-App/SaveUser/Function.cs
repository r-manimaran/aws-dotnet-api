using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SaveUser;

public class Function
{

    private readonly IAmazonDynamoDB _dynamoDbClient = new AmazonDynamoDBClient();
    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task<object> FunctionHandler(UserProfile profile, ILambdaContext context)
    {
       var item = new Dictionary<string, Amazon.DynamoDBv2.Model.AttributeValue>
       {
           ["UserId"] = new AttributeValue { S = Guid.NewGuid().ToString() },
           ["Name"] = new AttributeValue { S = profile.Name },
           ["Email"] = new AttributeValue { S = profile.Email },
           ["Role"] = new AttributeValue { S = profile.Role },
           ["CreatedAt"] = new AttributeValue { S = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") },
           ["Status"] = new AttributeValue { S = "PendingVerification" }
       };

        await _dynamoDbClient.PutItemAsync(new PutItemRequest
        {
            TableName = "UserProfiles",
            Item = item
        });
        return item;
    }
}

public class UserProfile
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
