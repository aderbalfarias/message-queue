namespace MessageQueue.Domain.Entities
{
    public class NServiceBusSettings
    {
        public string ProjectEndpoint { get; set; }

        public string SubscribeToEndpoint { get; set; }

        public string RouteToEndpoint { get; set; }

        public string PathToLog { get; set; }

        public int NumberOfRetries { get; set; }

        public string SendHeartbeatTo { get; set; }

        public string AuditProcessedMessagesTo { get; set; }

        public string SendFailedMessagesTo { get; set; }

        public string SendMetricDataToServiceControl { get; set; }

        public int SendMetricDataToServiceControlIntervalInMilliseconds { get; set; }

        public int RecoverabilityTimeIncreaseInSeconds { get; set; }

        public int SubscriptionCacheForInMinutes { get; set; }

        public bool UseRetry { get; set; } = false;

        public bool UseMetrics { get; set; } = false;
    }
}