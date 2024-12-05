
using FakeOrderPlacingSystem;
using Refit;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRefitClient<IOrderCreator>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:5215/api/"));
    
builder.Services.AddHostedService<OrderCreator>();
var app = builder.Build();

app.Run();
