using MessageProcessingAndAnomalyDetectionService.Data;

namespace MessageProcessingAndAnomalyDetectionService.Repositories.Intf;

public interface IServerStatisticsRepository
{
    Task AddAsync(ServerStatistics serverStatistics);
    Task<ServerStatistics?> GetLastAsync();
}