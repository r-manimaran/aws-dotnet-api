﻿using ApiService.Models;
using Microsoft.Extensions.Options;

namespace ApiService.Services;

public class DocumentUploadedEventHandler : IEventHandler<DocumentUploadedEvent>
{
    private readonly IEmailSender _emailSender;
    private readonly SESSettings _emailSettings;
    private readonly ILogger<DocumentUploadedEventHandler> _logger;

    public DocumentUploadedEventHandler(
        IEmailSender emailSender, 
        IOptions<SESSettings> emailSettings, 
        ILogger<DocumentUploadedEventHandler> logger)
    {
        _emailSender = emailSender;
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    public async Task HandleAsync(DocumentUploadedEvent @event, CancellationToken cancellationToken = default)
    {
        try
        {
            var subject = "New Document uploaded (Background Processing)";
            var body =
                $"A new document `{@event.OriginalFileName}` has been uploaded to S3 and processed in the background.";

            await _emailSender.SendEmailAsync(_emailSettings.AdminEmail, subject, body);

            _logger.LogInformation("Email sent successfully for document upload event:{EventId}, File:{FileName}",
                @event.EventId,
                @event.FileName);

        }
        catch (Exception ex)
        {
            _logger.LogError(
              ex,
              "Error processing document upload event: {EventId}, File: {FileName}",
              @event.EventId,
              @event.FileName);
            throw;
        }
    }
}
