using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;
using Amazon.SecretsManager;

namespace AccessSecretManagerApi;

public static class AppExtensionsMethods
{
    public static void AddAmazonSecretsManager(this IServiceCollection services)
    {
        services.AddAWSService<IAmazonSecretsManager>(
                  new AWSOptions
                  {
                      Region = RegionEndpoint.USEast1
                      // Region = RegionEndpoint.GetBySystemName(builder.Configuration["AwsSettings:Region"])
                  });                
    }

    public static void AddAmazonS3(this IServiceCollection services)
    {
        services.AddAWSService<IAmazonS3>(
                  new AWSOptions
                  {
                      Region = RegionEndpoint.USEast1
                      // Region = RegionEndpoint.GetBySystemName(builder.Configuration["AwsSettings:Region"])
                  });
    }

}
