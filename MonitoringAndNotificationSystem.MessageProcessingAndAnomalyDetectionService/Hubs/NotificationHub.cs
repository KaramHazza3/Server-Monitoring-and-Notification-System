using Microsoft.AspNetCore.SignalR;

namespace MessageProcessingAndAnomalyDetectionService.Hubs;

public class NotificationHub : Hub<IServerStatisticsClient>
{
    public async Task SendMessage(string message)
    {
        await Clients.All.AnomalyMessage(message);
    }
}