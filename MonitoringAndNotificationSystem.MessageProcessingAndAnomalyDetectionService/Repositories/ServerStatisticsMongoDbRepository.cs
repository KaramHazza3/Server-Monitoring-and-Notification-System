using MessageProcessingAndAnomalyDetectionService.Contexts;
using MessageProcessingAndAnomalyDetectionService.Data;
using MessageProcessingAndAnomalyDetectionService.Repositories.Intf;
using MongoDB.Driver;

namespace MessageProcessingAndAnomalyDetectionService.Repositories;

public class ServerStatisticsMongoDbRepository : IServerStatisticsRepository
{
    private readonly MongoDbContext _context;

    public ServerStatisticsMongoDbRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ServerStatistics serverStatistics)
    {
        await this._context.ServerStatistics.InsertOneAsync(serverStatistics);
    }

    public async Task<ServerStatistics?> GetLastAsync()
    {
        return await this._context.ServerStatistics
            .Find(FilterDefinition<ServerStatistics>.Empty)
            .SortByDescending(s => s.Timestamp)
            .FirstOrDefaultAsync();
    }
}