using api_lambda_deployment.Abstractions;
using api_lambda_deployment.Entities;
using api_lambda_deployment.Models;
using api_lambda_deployment.Services;

namespace api_lambda_deployment.Endpoints;

public class Post : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/shorten", async (ShortUrlRequest request,
                                        UrlShortnerService urlShorteningService,
                                        ApplicationDbContext dbContext,
                                        HttpContext httpContext) =>
        {
            if (!Uri.TryCreate(request.Url, UriKind.Absolute, out _))
            {
                return Results.BadRequest("Invalid URL");
            }

            var code  = await urlShorteningService.GenerateUniqueCode();
            var shortnedUrl = new ShortenedUrl
            {
                Id = Guid.NewGuid(),
                LongUrl = request.Url,
                ShortUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/api/{code}",
                Code = code,
                CreatedAt = DateTime.UtcNow              
                
            };
            dbContext.shortenedUrls.Add(shortnedUrl);
            await dbContext.SaveChangesAsync();

            return Results.Ok(shortnedUrl.ShortUrl);
        });
    }
}
