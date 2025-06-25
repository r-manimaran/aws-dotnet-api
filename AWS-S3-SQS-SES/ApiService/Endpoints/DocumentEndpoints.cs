using ApiService.Models;
using ApiService.Services;
using Microsoft.Extensions.Options;

namespace ApiService.Endpoints;

public static class DocumentEndpoints
{
    public static void MapDocumentsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/documents")
                       .WithTags("Amz")
                       .WithOpenApi();

        group.MapPost("upload", async (IFormFile file, IFileStorageService storageService, ILogger<Program> logger ) =>
        {
            if(file.Length == 0) return Results.BadRequest("No file provided");

            if(!file.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
            {
                return Results.BadRequest("Only Pdf files are allowed");
            }

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            try
            {
                
                var response = await storageService.UploadFileAsync(file, fileName);

                logger.LogInformation("File uploaded successfully.{FileName}", fileName);
                
                return Results.Ok(new
                {
                    Message = "File uploaded successfully to S3 bucket",
                    fileName = fileName
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());

                return Results.InternalServerError(new
                {
                    Message = ex.Message,
                    fileName = fileName
                });
            }
        }).DisableAntiforgery();


        group.MapPost("upload-and-email", async (IFormFile file, 
            IFileStorageService storageService, 
            IEmailSender emailSender,
            ILogger<Program> logger,
            IOptions<SESSettings> sesSettings) =>
        {
            if (file.Length == 0) return Results.BadRequest("No file provided");

            if (!file.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
            {
                return Results.BadRequest("Only Pdf files are allowed");
            }

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            try
            {
                var response = await storageService.UploadFileAsync(file, fileName);

                var subject = "New document uploaded";
                var body = $"A new document '{file.FileName}' has been uploaded to S3";

                await emailSender.SendEmailAsync(sesSettings.Value.AdminEmail,
                    subject,
                    body
                    );
                logger.LogInformation("File uploaded and Emailed successfully.{FileName}", fileName);

                return Results.Ok(new
                {
                    Message = "File uploaded successfully to S3 bucket",
                    fileName = fileName
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());

                return Results.InternalServerError(new
                {
                    Message = ex.Message,
                    fileName = fileName
                });
            }
        }).DisableAntiforgery();

        group.MapPost("upload-and-queue", async (IFormFile file,
            IFileStorageService storageService,
            IMessagePublisher messagePublisher,
            ILogger<Program> logger,
            IOptions<SqsSettings> sqsSettings) =>
        {
            if (file.Length == 0) return Results.BadRequest("No file provided");

            if (!file.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase))
            {
                return Results.BadRequest("Only Pdf files are allowed");
            }

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            try
            {
                var response = await storageService.UploadFileAsync(file, fileName);

                // create the Event
                var @event = new DocumentUploadedEvent
                {
                    FileName= fileName,
                    OriginalFileName = file.FileName,
                    CorrelationId = Guid.NewGuid().ToString()
                };
               
                await messagePublisher.PublishEventAsync(@event, sqsSettings.Value.QueueUrl);

                logger.LogInformation("File uploaded and event published successfully.{FileName}, EventId: {EventId}, CorrelationId: {CorrelationId}",
                    fileName,
                    @event.EventId,
                    @event.CorrelationId);

                return Results.Ok(new
                {
                    Message = "File uploaded and event queued for background processing",
                    fileName = fileName,
                    eventId = @event.EventId,
                    correlationId = @event.CorrelationId
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());

                return Results.InternalServerError(new
                {
                    Message = ex.Message,
                    fileName = fileName
                });
            }
        }).DisableAntiforgery();


    }
}
