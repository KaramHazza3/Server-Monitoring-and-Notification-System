using MessageProcessingAndAnomalyDetectionService.Config;
using MessageProcessingAndAnomalyDetectionService.Data;
using MessageProcessingAndAnomalyDetectionService.Services.AnomalyDetection.Strategies.Intf;
using Microsoft.Extensions.Options;

namespace MessageProcessingAndAnomalyDetectionService.Services.AnomalyDetection.Strategies;

public class MemoryHighUsageAlertStrategy : IAlertStrategy
{
    private readonly AnomalyDetectionConfig _anomalyDetectionConfig;

    public MemoryHighUsageAlertStrategy(IOptions<AnomalyDetectionConfig> anomalyDetectionConfig)
    {
        _anomalyDetectionConfig = anomalyDetectionConfig.Value;
    }

    public bool IsAlert(ServerStatistics serverStatistics, ServerStatistics previousServerStatistics)
    {
        return (serverStatistics.MemoryUsage / (serverStatistics.MemoryUsage + serverStatistics.AvailableMemory)) >
               _anomalyDetectionConfig.MemoryUsageThresholdPercentage;
    }

    public string GetAlertMessage()
    {
        return "High Usage Alert: Memory Usage exceeded threshold!";
    }
}