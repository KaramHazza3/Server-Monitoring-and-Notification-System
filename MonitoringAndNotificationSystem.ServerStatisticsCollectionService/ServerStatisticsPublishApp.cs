using Microsoft.Extensions.Options;
using MonitoringAndNotificationSystem.MessageBroker;
using ServerStatisticsCollectionService.Config;
using ServerStatisticsCollectionService.Services.ServerStatisticsCollector.Intf;

namespace ServerStatisticsCollectionService;

public class ServerStatisticsPublishApp
{
    private readonly ServerStatisticsConfig _serverStatisticsConfig;
    private readonly MessageBrokerConfig _messageBrokerConfig;
    private readonly IServerStatisticsCollector _serverStatisticsCollector;
    private readonly IMessageBroker _messageBroker;

    public ServerStatisticsPublishApp(IOptions<ServerStatisticsConfig> serverStatisticsConfig,
        IOptions<MessageBrokerConfig> messageBrokerConfig,
        IServerStatisticsCollector serverStatisticsCollector,
        IMessageBroker messageBroker)
    {
        _serverStatisticsConfig = serverStatisticsConfig.Value;
        _messageBrokerConfig = messageBrokerConfig.Value;
        _serverStatisticsCollector = serverStatisticsCollector;
        _messageBroker = messageBroker;
    }
    public async Task CollectAndPublishAsync()
    {
        var serverStatistics = await _serverStatisticsCollector.CollectStatisticsAsync(_serverStatisticsConfig.SamplingIntervalSeconds);
        await _messageBroker.PublishAsync($"{_messageBrokerConfig.RoutingKey}.{_serverStatisticsConfig.ServerIdentifier}",
            serverStatistics);
    }
}