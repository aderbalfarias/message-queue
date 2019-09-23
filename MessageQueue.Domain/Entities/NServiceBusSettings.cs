namespace MessageQueue.Domain.Entities
{
    public class NServiceBusSettings
    {
        public string CrsHandlerEndpoint { get; set; }

        public int NumberOfRetries { get; set; }

        public string SendHeartbeatTo { get; set; }

        public string AuditProcessedMessagesTo { get; set; }

        public string SendFailedMessagesTo { get; set; }

        public string SendMetricDataToServiceControl { get; set; }

        public int SendMetricDataToServiceControlIntervalInMilliseconds { get; set; }

        public int RecoverabilityTimeIncreaseInSeconds { get; set; }

        public int SubscriptionCacheForInMinutes { get; set; }
    }
}