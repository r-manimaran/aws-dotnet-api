namespace AmzSimpleEmail;

public class EmailSettings
{
    public const string ConfigurationSectionName = nameof(EmailSettings);
    public string FromAddress { get; set; }
}
