namespace Webapi.JudgingQueue
{
    public class ServiceBusSettings
    {
        public string ConnectionString { get; set; }

        public string SubmissionQueueName { get; set; }
    }
}
