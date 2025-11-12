using Microsoft.AspNetCore.Mvc;

namespace BlogsWebApi;

public static class Endpoints
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapGet("/posts/{id}", async (IBlogApi blogApi, int id) =>
            await blogApi.GetPostAsync(id));

        app.MapGet("/posts", async(int? userId, IBlogApi blogApi) =>
            await blogApi.GetPostsAsync(userId));

        app.MapPost("/posts", async([FromBody] Post post, IBlogApi blogApi) =>
            await blogApi.CreatePostAsync(post));

        app.MapPut("/posts/{id}", async (int id, [FromBody] Post post, IBlogApi blogApi) =>
            await blogApi.UpdatePostAsync(id, post));

        app.MapDelete("/posts/{id}", async (int id, IBlogApi blogApi) =>
            await blogApi.DeletePostAsync(id));

    }
}
