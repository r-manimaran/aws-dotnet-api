using Amazon;
using Amazon.CDK.AWS.SQS;

var builder = DistributedApplication.CreateBuilder(args);

var awsSdKConfig = builder.AddAWSSDKConfig().WithRegion(RegionEndpoint.USEast1);
var awsResources = builder.AddAWSCDKStack("OrderServiceStack")
    .WithReference(awsSdKConfig);

// Flow
// OrderService --> Place Order--> Submit to OrdersTopic --> Notify to BillingQueue and Shipping Queue. BillingQue have a DLQ

// AWS DLQ's for the SQS Queues
var billingQueueDLQ = awsResources.AddSQSQueue("OrdersBillingQueueDLQ");

//IQueueProps props = new QueueProps()
//{
//    DeadLetterQueue = new DeadLetterQueue() 
//    {
//        Queue = (Queue) billingQueueDLQ,
//        MaxReceiveCount = 3
//    }
//};

// AWS SQS Queues
var billingQueue = awsResources.AddSQSQueue("OrdersBillingQueue");


var shippingQueue = awsResources.AddSQSQueue("OrdersShippingQueue");

// AWS Topic for Orders
var ordersTopic =  awsResources.AddSNSTopic("OrdersTopic")
                            .AddSubscription(billingQueue)
                            .AddSubscription(shippingQueue);

builder.AddProject<Projects.OrderService>("orderservice")
       .WithReference(ordersTopic)
       .WithReference(awsSdKConfig);

builder.AddProject<Projects.BillingProcessor>("billingservice")
       .WithReference(awsSdKConfig)
       .WithReference(billingQueueDLQ)
       .WithReference(billingQueue);

builder.AddProject<Projects.ShippingProcessor>("shippingservice")
    .WithReference(shippingQueue)
    .WithReference(awsSdKConfig);

builder.Build().Run();
