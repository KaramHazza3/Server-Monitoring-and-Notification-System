namespace MessageProcessingAndAnomalyDetectionService.Hubs;

public interface IServerStatisticsClient
{
    Task AnomalyMessage(string message);
}