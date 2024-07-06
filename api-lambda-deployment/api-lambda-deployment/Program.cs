using System.Reflection;
using api_lambda_deployment;
using api_lambda_deployment.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Host the Web API in Lambda, Add the below line
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

//Get the Connection String from the Environment Variable
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (connectionString == null)
{
    throw new InvalidOperationException("Connection string 'Database' not found.");
}
// Log Connection string
Console.WriteLine($"Connection string: {connectionString}");
//Add the DbContext
builder.Services.AddDbContext<ApplicationDbContext>(opt => opt.UseNpgsql(connectionString));

//Add the Service in the app lifecycle
builder.Services.AddScoped<UrlShortnerService>();

//Add the endpoints
builder.Services.AddEndPoints(typeof(Program).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    System.Console.WriteLine("Applying Migration");
    //Apply the Migration in development Environment
    app.ApplyDbMigration();
}

app.UseHttpsRedirection();


app.MapEndPoints();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
}).WithTags("WeatherApi");


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
