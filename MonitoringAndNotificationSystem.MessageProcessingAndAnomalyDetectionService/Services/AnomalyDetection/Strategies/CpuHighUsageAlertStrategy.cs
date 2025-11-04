using MessageProcessingAndAnomalyDetectionService.Config;
using MessageProcessingAndAnomalyDetectionService.Data;
using MessageProcessingAndAnomalyDetectionService.Services.AnomalyDetection.Strategies.Intf;
using Microsoft.Extensions.Options;

namespace MessageProcessingAndAnomalyDetectionService.Services.AnomalyDetection.Strategies;

public class CpuHighUsageAlertStrategy : IAlertStrategy
{
    private readonly AnomalyDetectionConfig _anomalyDetectionConfig;

    public CpuHighUsageAlertStrategy(IOptions<AnomalyDetectionConfig> anomalyDetectionConfig)
    {
        _anomalyDetectionConfig = anomalyDetectionConfig.Value;
    }

    public bool IsAlert(ServerStatistics serverStatistics, ServerStatistics previousServerStatistics)
    {
        return (serverStatistics.CpuUsage > _anomalyDetectionConfig.CpuUsageThresholdPercentage);
    }

    public string GetAlertMessage()
    {
        return "High Usage Alert: CPU Usage exceeded threshold!";
    }
}