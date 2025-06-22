using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using AmzSimpleEmail.Templates;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace AmzSimpleEmail.Endpoints;

public static class AmazonSESEndpoints
{
    public static void MapSESEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/email", async(string email, 
                            IAmazonSimpleEmailService emailService, 
                            IOptions<EmailSettings> settings) =>
        {
            var request = new SendEmailRequest
            {
                Source = settings.Value.FromAddress,
                Destination = new Destination
                {
                    ToAddresses = new List<string> { email },
                    //BccAddresses = new List<string> { email },
                    //CcAddresses = new List<string> { email }
                },
                Message = new Message
                {
                    Subject = new Content("Welcome to Email Functionality from Amazon SES using .Net"),
                    Body = new Body
                    {
                        // Text = new Content("This is a test email sent using Amazon SES."),
                        Html = new Content(
                            """
                            <html>
                                <body style="font-family: Arial, sans-serif; font-size: 16px; color: #333;">
                                    <h1>Welcome to Email Functionality</h1>
                                    <p>This is a test email sent using Amazon SES.</p>
                                    <p>Thank you for using our service!</p>
                                 </body>
                             </html>
                            """)
                    }
                }
            };

            try
            {
                var response = await emailService.SendEmailAsync(request);
                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    return Results.Ok(
                        new { 
                            messageId = response.MessageId,
                            statusCode = response.HttpStatusCode
                            });
                }
                else
                {
                    return Results.Problem("Failed to send email.", statusCode: 500);
                }
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, statusCode: 500);
            }

        }).WithName("SendEmail")
          .WithTags("Amazon SES Endpoints");

        endpoints.MapPost("/email/initialize-templates", async (IAmazonSimpleEmailService emailService) =>
        {
            try
            {
                await EmailTemplates.InitializeEmailTemplates(emailService);
                return Results.Ok("Email templates initialized successfully.");
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, statusCode: 500);
            }
        }).WithName("InitializeEmailTemplates")
          .WithTags("Amazon SES Endpoints");

        endpoints.MapPost("email/welcome", async(string email, IAmazonSimpleEmailService emailService, IOptions<EmailSettings> settings)=>
        {
            var request = new SendTemplatedEmailRequest
            {
                Source = settings.Value.FromAddress,
                Destination = new Destination
                {
                    ToAddresses = new List<string> { email }
                },
                Template = EmailTemplates.WelcomeEmailTemplate,
                //TemplateData = $"{{\"user_name\":\"{email}\"}}"
                TemplateData = JsonSerializer.Serialize(new { user_name = email }) // Using System.Text.Json for serialization
            };
            try
            {
                var response = await emailService.SendTemplatedEmailAsync(request);
                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    return Results.Ok(new { messageId = response.MessageId, statusCode = response.HttpStatusCode });
                }
                else
                {
                    return Results.Problem("Failed to send welcome email.", statusCode: 500);
                }
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, statusCode: 500);
            }
        }).WithName("SendWelcomeEmail")
          .WithTags("Amazon SES Endpoints");



        endpoints.MapPost("email/newsletter", async(string email, 
            IAmazonSimpleEmailService emailService, 
            IOptions<EmailSettings> settings) =>
        {
            var request = new SendTemplatedEmailRequest
            {
                Source = settings.Value.FromAddress,
                Destination = new Destination
                {
                    ToAddresses = new List<string> { email }
                },
                Template = EmailTemplates.NewsletterTemplate,
                TemplateData = JsonSerializer.Serialize(new 
                { 
                    user_name = email.Split('@')[0],
                    newsletter_name = "dotnet-devs",
                    newsletter_title = "Monthly Newsletter",
                    newsletter_content = "Stay tuned for our latest updates and offers!",
                    articles = new[]
                    {
                        new { title="New Dashboard Features", content="Explore the new features in our dashboard." },
                        new { title="Upcoming Events", content="Join us for our upcoming webinars and events." },
                        new { title="Special Offers", content="Check out our special offers for this month." }
                    }
                })
            }; // Using System.Text.Json for serialization

            try
            {
                var response = await emailService.SendTemplatedEmailAsync(request);
                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    return Results.Ok(new { messageId = response.MessageId, statusCode = response.HttpStatusCode });
                }
                else
                {
                    return Results.Problem("Failed to send welcome email.", statusCode: 500);
                }
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message, statusCode: 500);
            }

        }).WithName("SendNewsletterEmail")
          .WithTags("Amazon SES Endpoints");
    }

    
}
