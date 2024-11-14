using System.ComponentModel.DataAnnotations;

namespace AccessSecretManagerApi;

public class AwsSettings
{
    [Required]
    public string AccessKey { get; set; } = string.Empty;
    [Required]
    public string SecretKey { get; set; } = string.Empty;
    [Required]
    public string Region { get; set; } =string.Empty;
    [Required]
    public string BucketName { get; set; } = string.Empty;
}
