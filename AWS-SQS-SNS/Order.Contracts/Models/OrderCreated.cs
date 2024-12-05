namespace Order.Contracts.Models;

public record  OrderCreated (Guid OrderId, Guid CustomerId, decimal Total, DateTime CreatedAt);
