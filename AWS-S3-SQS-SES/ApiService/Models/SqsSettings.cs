namespace ApiService.Models
{
    public class SqsSettings
    {
        public static string SectionName = nameof(SqsSettings);
        public string QueueUrl { get; set; } = string.Empty;
    }
}
