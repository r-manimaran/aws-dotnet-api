using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib;

public class OrderCreatedEvent
{
    public string OrderId { get; set; }
    public string CustomerId { get; set; }
    public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    public double TotalAmount { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.Now;
}
