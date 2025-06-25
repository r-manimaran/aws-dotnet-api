namespace ApiService.Models
{
    public class SESSettings
    {
        public static string SectionName = nameof(SESSettings);

        public string AdminEmail { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
    }
}
