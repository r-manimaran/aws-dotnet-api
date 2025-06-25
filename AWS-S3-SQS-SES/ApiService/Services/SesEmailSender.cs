using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using ApiService.Models;
using Microsoft.Extensions.Options;

namespace ApiService.Services;

public class SesEmailSender : IEmailSender
{
    private readonly IAmazonSimpleEmailService _sesClient;
    private readonly SESSettings _sesSettings;

    public SesEmailSender(IAmazonSimpleEmailService sesClient, IOptions<SESSettings> sesSettings)
    {
        _sesClient = sesClient;
        _sesSettings = sesSettings.Value;
    }
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var sendRequest = new SendEmailRequest
        {
            Source = _sesSettings.SenderEmail,
            Destination = new Destination
            {
                ToAddresses = [to]
            },
            Message = new Message
            {
                Subject = new Content(subject),
                Body = new Body
               {
                   Html = new Content(body)
               }
            }
        };

        await _sesClient.SendEmailAsync(sendRequest);
    }
}
