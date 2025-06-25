namespace ApiService.Models;

public class S3Settings
{
    public static string SectionName = nameof(S3Settings);
    public string BucketName { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
}
