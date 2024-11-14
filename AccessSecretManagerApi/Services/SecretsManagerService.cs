using System.Text.Json;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.Extensions.Caching.Memory;

namespace AccessSecretManagerApi;

public class SecretsManagerService
{
    private readonly IAmazonSecretsManager _secretManager;
    private readonly IMemoryCache _cache;
    private readonly ILogger<SecretsManagerService> _logger;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(1);

    public SecretsManagerService(IAmazonSecretsManager secretManager,
                                     IMemoryCache cache,
                                     ILogger<SecretsManagerService> logger)
    {
        this._secretManager = secretManager;
        this._cache = cache;
        this._logger = logger;
        this._cache = cache;
    }

    public async Task<AwsSettings> GetAwsSecretsAsync(string secretName)
    {
        var cacheKey = $"GetAwsSecretsAsync_{secretName}";
        bool isCacheMissed = false;
        // If not in Cache, get from AWS and store it in Cache
        if (!_cache.TryGetValue(cacheKey, out AwsSettings? awsSettings))
        {
            _logger.LogInformation($"Cache miss for {cacheKey}");
             isCacheMissed = true;
            // build the request to get from Secrets Manager
            var request  = new GetSecretValueRequest { SecretId = secretName };
            var response = await _secretManager.GetSecretValueAsync(request);

            awsSettings =JsonSerializer.Deserialize<AwsSettings>(response.SecretString);

            //set it in cache
            _cache.Set(cacheKey, awsSettings, _cacheDuration);
        }
        if(!isCacheMissed)
            _logger.LogInformation($"Cache hit for {cacheKey}");

        return awsSettings;
    }

}
