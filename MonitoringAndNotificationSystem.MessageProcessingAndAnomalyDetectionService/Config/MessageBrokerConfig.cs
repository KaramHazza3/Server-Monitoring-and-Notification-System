namespace MessageProcessingAndAnomalyDetectionService.Config;

public class MessageBrokerConfig
{
    public string QueueName { get; set; } = string.Empty;
    public string ExchangeName { get; set; } = string.Empty;
    public string RoutingKey { get; set; } = string.Empty;
    public string Uri { get; set; } = string.Empty;
    public string ServerName { get; set; } = string.Empty;
}