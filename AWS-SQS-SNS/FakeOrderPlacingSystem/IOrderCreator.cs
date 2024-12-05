using System;
using Order.Contracts.Models;
using Refit;

namespace FakeOrderPlacingSystem;

public interface IOrderCreator
{
    [Post("/Order")]
    Task<PublishOrder> CreateOrder([Body] OrderRequest orderRequest);
}
