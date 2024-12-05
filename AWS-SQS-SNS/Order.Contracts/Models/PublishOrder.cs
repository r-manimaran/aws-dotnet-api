using System;

namespace Order.Contracts.Models;

public class PublishOrder
{
    public Guid OrderId { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; } 
    public decimal TotalAmount { get; set; }
    public IEnumerable<OrderItemRequest> Items { get; set; }
}