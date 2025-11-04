using MessageProcessingAndAnomalyDetectionService.Config;
using MessageProcessingAndAnomalyDetectionService.Data;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MessageProcessingAndAnomalyDetectionService.Contexts;

public class MongoDbContext
{
    private readonly IMongoDatabase _mongoDatabase;
    private readonly MongoDbConfig _mongoDbConfig;
    
    public MongoDbContext(IOptions<MongoDbConfig> mongoDbConfig)
    {
        _mongoDbConfig = mongoDbConfig.Value;
        var client = new MongoClient(_mongoDbConfig.ConnectionString);
        _mongoDatabase = client.GetDatabase(_mongoDbConfig.DatabaseName);
    }

    public IMongoCollection<ServerStatistics> ServerStatistics => _mongoDatabase.GetCollection<ServerStatistics>("ServerStatistics");
}