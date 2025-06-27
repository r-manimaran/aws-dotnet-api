namespace MessagingApi.Contracts;

public class OrderCreatedEvent
{
    public string OrderId { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public List<string> Items { get; set; } = new List<string>();
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
}
