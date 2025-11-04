using MessageProcessingAndAnomalyDetectionService.Config;
using MessageProcessingAndAnomalyDetectionService.Data;
using MessageProcessingAndAnomalyDetectionService.Services.AnomalyDetection.Strategies.Intf;
using Microsoft.Extensions.Options;

namespace MessageProcessingAndAnomalyDetectionService.Services.AnomalyDetection.Strategies;

public class CpuUsageAnomalyAlertStrategy : IAlertStrategy
{
    private readonly AnomalyDetectionConfig _anomalyDetectionConfig;

    public CpuUsageAnomalyAlertStrategy(IOptions<AnomalyDetectionConfig> anomalyDetectionConfig)
    {
        _anomalyDetectionConfig = anomalyDetectionConfig.Value;
    }

    public bool IsAlert(ServerStatistics serverStatistics, ServerStatistics previousServerStatistics)
    {
        return serverStatistics.CpuUsage > (previousServerStatistics.CpuUsage * (1 + _anomalyDetectionConfig.CpuUsageAnomalyThresholdPercentage));
    }

    public string GetAlertMessage()
    {
        return "Alert Alert: Sudden increase in Cpu Usage!";
    }
}