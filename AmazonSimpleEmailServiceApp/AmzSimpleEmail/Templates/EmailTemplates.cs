using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

namespace AmzSimpleEmail.Templates;

public static class EmailTemplates
{
    public const string WelcomeEmailTemplate = "WelcomeTemplate";
    public const string NewsletterTemplate = "NewsletterTemplate";

    public static async Task InitializeEmailTemplates(IAmazonSimpleEmailService emailService)
    {
    
        var templates = new List<string> { WelcomeEmailTemplate, NewsletterTemplate };
        foreach (var template in templates)
        {
            var content = GetTemplateContent(template);
            var request = new CreateTemplateRequest
            {
                Template = new Template
                {
                    TemplateName = template,
                    HtmlPart = content,
                    SubjectPart = GetTemplateSubject(template),
                    TextPart = "This is a test email sent using Amazon SES."
                }
            };
            try
            {
                // Check if the template already exists before creating it
                await emailService.DeleteTemplateAsync(new DeleteTemplateRequest { TemplateName = template });
                // Create the template
                await emailService.CreateTemplateAsync(request);
            }
            catch (Exception ex)
            {
                // Handle exceptions, e.g., log them or rethrow
                Console.WriteLine($"Error creating template {template}: {ex.Message}");
            }
        }
    }

    public static string GetTemplateSubject(string templateName)
    {
        return templateName switch
        {
            WelcomeEmailTemplate => "Welcome to Email Functionality from Amazon SES using .Net, {{user_name}}!",
            NewsletterTemplate => "Your Latest Newsletter from {{newsletter_name}}",
            _ => throw new ArgumentException("Invalid template name.", nameof(templateName))
        };
    }
    public static string GetTemplateContent(string templateName)
    {
        return templateName switch
        {
            WelcomeEmailTemplate => """
                <html>
                    <body style="font-family: Arial, sans-serif; font-size: 16px; color: #333;">
                        <h1>Welcome to Email Functionality {{user_name}}</h1>
                        <p>This is a test email sent using Amazon SES.</p>
                        <p>Thank you for using our service!</p>
                    </body>
                </html>
                """,
            NewsletterTemplate => """
                Hello {{user_name}},
                Here is your latest newsletter!

                {{newsletter_content}}

                Featured Articles:
                {{#each articles}}
                  - {{this.title}}: {{this.content}}
                {{/each}}

                Stay tuned for more updates!

                Best regards,
                Your Newsletter Team.
                """,
            _ => throw new ArgumentException("Invalid template name.", nameof(templateName))
        };
    }
}
