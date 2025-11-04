using MessageProcessingAndAnomalyDetectionService.Data;

namespace MessageProcessingAndAnomalyDetectionService.Services.AnomalyDetection.Strategies.Intf;

public interface IAlertStrategy
{
    bool IsAlert(ServerStatistics serverStatistics, ServerStatistics previousServerStatistics);
    string GetAlertMessage();
}