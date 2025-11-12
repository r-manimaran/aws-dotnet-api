using BlogsWebApi;
using Refit;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRefitClient<IBlogApi>()
    .ConfigureHttpClient(client => client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com"));

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapEndpoints();

app.Run();
