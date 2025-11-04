using MessageProcessingAndAnomalyDetectionService.Config;
using MessageProcessingAndAnomalyDetectionService.Data;
using MessageProcessingAndAnomalyDetectionService.Repositories.Intf;
using MessageProcessingAndAnomalyDetectionService.Services.AnomalyDetection.Intf;
using Microsoft.Extensions.Options;
using MonitoringAndNotificationSystem.MessageBroker;

namespace MessageProcessingAndAnomalyDetectionService;

public class AnomalyDetectionApp
{
    private readonly IMessageBroker _messageBroker;
    private readonly IServerStatisticsRepository _serverStatisticsRepository;
    private readonly IAnomalyDetectionService _anomalyDetectionService;
    private readonly MessageBrokerConfig _messageBrokerConfig;
    
    public AnomalyDetectionApp(IMessageBroker messageBroker, 
        IOptions<MessageBrokerConfig> messageBrokerConfig,
        IServerStatisticsRepository serverStatisticsRepository,
        IAnomalyDetectionService anomalyDetectionService)
    {
        _messageBroker = messageBroker;
        _messageBrokerConfig = messageBrokerConfig.Value;
        _serverStatisticsRepository = serverStatisticsRepository;
        _anomalyDetectionService = anomalyDetectionService;
    }

    public async Task ConsumeAndProcessMessage()
    {
        try
        {
            await _messageBroker.ConsumeAsync<ServerStatistics>($"{_messageBrokerConfig.RoutingKey}.*",
                ProcessMessageAsync);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.Message}");
        }
    }

    private async Task ProcessMessageAsync(ServerStatistics serverStatistics)
    {
        await this._anomalyDetectionService.CheckAndSendAnomalyAlertAsync(serverStatistics);
        await this._serverStatisticsRepository.AddAsync(serverStatistics);
    }
}