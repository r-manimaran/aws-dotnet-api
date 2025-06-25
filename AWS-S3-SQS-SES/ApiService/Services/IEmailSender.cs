namespace ApiService.Services;

public interface IEmailSender
{
    Task SendEmailAsync(string adminEmail, string subject, string body);
}
