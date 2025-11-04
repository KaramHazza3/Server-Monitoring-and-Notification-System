using MessageProcessingAndAnomalyDetectionService.Config;
using MessageProcessingAndAnomalyDetectionService.Data;
using MessageProcessingAndAnomalyDetectionService.Services.AnomalyDetection.Strategies.Intf;
using Microsoft.Extensions.Options;

namespace MessageProcessingAndAnomalyDetectionService.Services.AnomalyDetection.Strategies;

public class MemoryUsageAlertStrategy : IAlertStrategy
{
    private readonly AnomalyDetectionConfig _anomalyDetectionConfig;
    public MemoryUsageAlertStrategy(IOptions<AnomalyDetectionConfig> anomalyDetectionConfig)
    {
        _anomalyDetectionConfig = anomalyDetectionConfig.Value;
    }
    public bool IsAlert(ServerStatistics serverStatistics, ServerStatistics previousServerStatistics)
    {
        return serverStatistics.MemoryUsage > (previousServerStatistics.MemoryUsage *
                                               (1 + _anomalyDetectionConfig.MemoryUsageAnomalyThresholdPercentage));
    }

    public string GetAlertMessage()
    {
        return "Anomaly Alert: Sudden increase in Memory Usage!";
    }
}