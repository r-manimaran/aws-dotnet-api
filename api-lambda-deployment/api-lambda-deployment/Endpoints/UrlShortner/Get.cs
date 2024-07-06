using api_lambda_deployment.Abstractions;
using Microsoft.EntityFrameworkCore;


namespace api_lambda_deployment.Endpoints;

public class Get : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/{code}", async (string code, ApplicationDbContext dbContext) =>
        {
            var shortnedUrl = await dbContext.shortenedUrls.FirstOrDefaultAsync(x => x.Code == code);
            if (shortnedUrl == null)
            {
                return Results.NotFound();
            }
            return Results.Redirect(shortnedUrl.LongUrl);
        });
    }
}