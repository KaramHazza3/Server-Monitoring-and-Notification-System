using MessageProcessingAndAnomalyDetectionService.Data;
using MessageProcessingAndAnomalyDetectionService.Hubs;
using MessageProcessingAndAnomalyDetectionService.Repositories.Intf;
using MessageProcessingAndAnomalyDetectionService.Services.AnomalyDetection.Intf;
using MessageProcessingAndAnomalyDetectionService.Services.AnomalyDetection.Strategies.Intf;
using Microsoft.AspNetCore.SignalR;

namespace MessageProcessingAndAnomalyDetectionService.Services.AnomalyDetection;

public class AnomalyDetectionService : IAnomalyDetectionService
{
    private IHubContext<NotificationHub, IServerStatisticsClient> _hubContext;
    private IServerStatisticsRepository _serverStatisticsRepository;
    private IEnumerable<IAlertStrategy> _alertStrategies;
    
    public AnomalyDetectionService(IHubContext<NotificationHub, IServerStatisticsClient> hubContext,
        IServerStatisticsRepository serverStatisticsRepository,
        IEnumerable<IAlertStrategy> alertStrategies)
    {
        _hubContext = hubContext;
        _serverStatisticsRepository = serverStatisticsRepository;
        _alertStrategies = alertStrategies;
    }
    
    public async Task CheckAndSendAnomalyAlertAsync(ServerStatistics currentServerStatistics)
    {
        var latestServerStatistics = await _serverStatisticsRepository.GetLastAsync() ?? currentServerStatistics;

        foreach (var alertStrategy in _alertStrategies)
        {
            if (alertStrategy.IsAlert(currentServerStatistics, latestServerStatistics))
            {
                await _hubContext.Clients.All.AnomalyMessage(alertStrategy.GetAlertMessage());
            }
        }
    }
}