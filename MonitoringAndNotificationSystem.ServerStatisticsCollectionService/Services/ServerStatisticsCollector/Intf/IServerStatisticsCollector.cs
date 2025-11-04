using ServerStatisticsCollectionService.Data;

namespace ServerStatisticsCollectionService.Services.ServerStatisticsCollector.Intf;

public interface IServerStatisticsCollector
{
    Task<ServerStatistics> CollectStatisticsAsync(int intervalSeconds);
}