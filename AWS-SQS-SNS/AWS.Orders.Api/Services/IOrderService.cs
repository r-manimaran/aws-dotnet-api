using System;
using Order.Contracts.Models;

namespace AWS.Orders.Api.Services;

public interface IOrderService
{
    Task<PublishOrder> CreateOrderAndPublish(OrderRequest newOrder);
}
