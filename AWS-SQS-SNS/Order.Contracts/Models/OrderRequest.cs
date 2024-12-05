using System;

namespace Order.Contracts.Models;

public record OrderRequest (string CustomerId, IEnumerable<OrderItemRequest> Items);
