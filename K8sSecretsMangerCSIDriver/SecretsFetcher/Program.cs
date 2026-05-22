using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using System.Text.Json;
using System.Threading.Tasks;

namespace SecretsFetcher;

public class Program
{
    public static async Task Main(string[] args)
    {
        string secretName = Environment.GetEnvironmentVariable("SECRET_NAME") ?? "mydb-creds";
        string region = Environment.GetEnvironmentVariable("AWS_REGION") ?? "us-east-1";

        IAmazonSecretsManager amazonSecretsManager = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region));

        GetSecretValueRequest request = new GetSecretValueRequest
        {
            SecretId = secretName,
            VersionStage = "AWSCURRENT"
        };

        GetSecretValueResponse response;

        try
        {
            response = await  amazonSecretsManager.GetSecretValueAsync(request);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving secret: {ex.Message}");
            throw;
        }

        string secret = response.SecretString;
        Console.WriteLine("Secret retrieved successfully (content hidden for security)");
    }
}
