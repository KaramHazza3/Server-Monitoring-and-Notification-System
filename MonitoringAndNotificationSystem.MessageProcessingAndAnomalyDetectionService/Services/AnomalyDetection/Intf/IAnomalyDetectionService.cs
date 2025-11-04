using MessageProcessingAndAnomalyDetectionService.Data;

namespace MessageProcessingAndAnomalyDetectionService.Services.AnomalyDetection.Intf;

public interface IAnomalyDetectionService
{
    public Task CheckAndSendAnomalyAlertAsync(ServerStatistics statistics);
}