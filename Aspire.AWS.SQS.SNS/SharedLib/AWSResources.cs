using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib;

public class AWSResources
{
    public const string SectionName = "AWS:Resources";
    public BillingQueue? OrdersBillingQueue { get; set; } = new BillingQueue();

    public ShippingQueue? OrdersShippingQueue { get; set; } = new ShippingQueue();

    public TopicResource? OrdersTopic { get; set; } = new TopicResource();

    public class ShippingQueue
    {
        public string QueueUrl { get; set; } = string.Empty;
    }

    public class BillingQueue
    {
        public string QueueUrl { get; set; } = string.Empty;
    }
    public class TopicResource
    {
        public string TopicArn { get; set; } = string.Empty;
        
    }
}
