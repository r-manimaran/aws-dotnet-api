using System;

namespace Order.Contracts.Models;

public record OrderItemRequest(string ProductId, int Quantity, decimal Price);