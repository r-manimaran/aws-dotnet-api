namespace MessagingApi
{
    public class AWSResources
    {
        public static string SectionName=nameof(AWSResources);

        public string SQSOrderQueueUrl { get; set; } =string.Empty;

        public string SNSOrderTopicUrl { get; set; } =string.Empty;
    }
}
