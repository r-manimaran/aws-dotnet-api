using Amazon.Lambda.Core;
using Sharedlib;
using SharedLib;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace GetGithubContent;

public class Function
{

    private static readonly HttpClient _client = new();

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns></returns>
    public async Task<ReleaseGitHubContentOutput> FunctionHandler(ReleaseWorkflowInput input, ILambdaContext context)
    {
        var token = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
        var apiBase = Environment.GetEnvironmentVariable("GITHUB_API_URL") ?? "https://api.github.com";

        if (string.IsNullOrWhiteSpace(token))
            throw new Exception("Missing GITHUB_TOKEN");

        if (!_client.DefaultRequestHeaders.UserAgent.Any())
            _client.DefaultRequestHeaders.UserAgent.ParseAdd("aws-lambda-github-client");

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var parsed = ParseReleaseUrl(input.ReleaseUrl);

        var tagMeta = await GetJson($"{apiBase}/repos/{parsed.Owner}/{parsed.Repo}/git/ref/tags/{parsed.Tag}");

        context.Logger.LogLine($"Tag Meta: {tagMeta}");

        var obj = tagMeta.GetProperty("object");
        var objType = obj.GetProperty("type").GetString();
        var objSha = obj.GetProperty("sha").GetString();

        string commitSha;

        if (string.Equals(objType, "tag", StringComparison.OrdinalIgnoreCase))
        {
            // annotated tag -> resolve tag object to get the commit sha
            var tagObject = await GetJson($"{apiBase}/repos/{parsed.Owner}/{parsed.Repo}/git/tags/{objSha}");
            commitSha = tagObject.GetProperty("object").GetProperty("sha").GetString();
        }
        else
        {
            // lightweight tag -> points directly to commit
            commitSha = objSha;
        }
        context.Logger.LogLine($"Commit SHA: {commitSha}");

        var commitData = await GetJson($"{apiBase}/repos/{parsed.Owner}/{parsed.Repo}/commits/{commitSha}");

        context.Logger.LogLine($"Commit Data: {commitData}");

        var tags = await GetJson($"{apiBase}/repos/{parsed.Owner}/{parsed.Repo}/tags");

        context.Logger.LogLine($"Tags: {tags}");

        var previousTag = tags.EnumerateArray()
            .Select(t => t.GetProperty("name").GetString())
            .Where(t => t != parsed.Tag)
            .FirstOrDefault();

        context.Logger.LogLine($"Previous Tag: {previousTag}");

        List<ReleaseCommitItem> history = new();

        if (!string.IsNullOrEmpty(previousTag))
        {
            var compareData = await GetJson($"{apiBase}/repos/{parsed.Owner}/{parsed.Repo}/compare/{previousTag}...{parsed.Tag}");
            context.Logger.LogLine($"Compare Data: {compareData}");
            history = compareData.GetProperty("commits").EnumerateArray()
                .Select(c => new ReleaseCommitItem
                {
                    Sha = c.GetProperty("sha").GetString(),
                    Message = c.GetProperty("commit").GetProperty("message").GetString(),
                    Author = c.GetProperty("commit").GetProperty("author").GetProperty("name").GetString(),
                    Date = c.GetProperty("commit").GetProperty("author").GetProperty("date").GetDateTime()
                })
                .ToList();
        }
        context.Logger.LogLine($"Commit History Count: {history.Count}");
        return new ReleaseGitHubContentOutput
        {
            Tag = parsed.Tag,
            CommitSha = commitSha,
            CommitMessage = commitData.GetProperty("commit").GetProperty("message").GetString(),
            CommitAuthor = commitData.GetProperty("commit").GetProperty("author").GetProperty("name").GetString(),
            CommitDate = commitData.GetProperty("commit").GetProperty("author").GetProperty("date").GetDateTime(),
            PreviousTag = previousTag,
            CommitHistory = history
        };
    }

    private (string Owner, string Repo, string Tag) ParseReleaseUrl(string url)
    {
        
        var regex = new Regex(@"github\.com\/(.+?)\/(.+?)\/releases\/tag\/(.+)$");
        var match = regex.Match(url);
        if (!match.Success) //|| match.Groups.Count != 4
        {
            throw new ArgumentException("Invalid GitHub release URL format.");
        }

        return (match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value);

    }

    private async Task<JsonElement> GetJson(string url)
    { 
        var res = await _client.GetAsync(url);
        
        var jsonContent = await res.Content.ReadAsStringAsync();
        if (!res.IsSuccessStatusCode)
        {
            // Include body for easier debugging of 404s / permissions
            throw new HttpRequestException($"Request to {url} failed with {(int)res.StatusCode} {res.ReasonPhrase}: {jsonContent}");
        }
        return JsonSerializer.Deserialize<JsonElement>(jsonContent);
    }
}
