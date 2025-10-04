using OrderService.Service;
using SharedLib;
namespace OrderService.Endpoints;

public static class OrdersEndpoint
{
    public static void MapOrdersEndpoint(this IEndpointRouteBuilder app)
    {

        app.MapPost("/orders", async (OrderDto request, OrdersService service, ILogger<OrderDto> logger) =>
        {
            logger.LogInformation("Order received: {Order}", request);

            if(request.Items.Count == 0)
            {
                return Results.BadRequest("No items in order. Order must contain at least one item.");
            }
            request.Id = Guid.CreateVersion7().ToString();
            request.OrderDate = DateTime.Now;

            await service.CreateOrder(request);

            return Results.Created($"/orders/{request.Id}", request);
            
        });


    }
}
