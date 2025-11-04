using MessageProcessingAndAnomalyDetectionService.BackgroundServices;
using MessageProcessingAndAnomalyDetectionService.Config;
using MessageProcessingAndAnomalyDetectionService.Contexts;
using MessageProcessingAndAnomalyDetectionService.Hubs;
using MessageProcessingAndAnomalyDetectionService.Repositories;
using MessageProcessingAndAnomalyDetectionService.Repositories.Intf;
using MessageProcessingAndAnomalyDetectionService.Services.AnomalyDetection;
using MessageProcessingAndAnomalyDetectionService.Services.AnomalyDetection.Intf;
using MessageProcessingAndAnomalyDetectionService.Services.AnomalyDetection.Strategies;
using MessageProcessingAndAnomalyDetectionService.Services.AnomalyDetection.Strategies.Intf;
using Microsoft.Extensions.Options;
using MonitoringAndNotificationSystem.MessageBroker;

namespace MessageProcessingAndAnomalyDetectionService;

class Program
{
    static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddUserSecrets<Program>()
            .AddEnvironmentVariables();

        builder.Services.AddSignalR();

        builder.Services.Configure<MessageBrokerConfig>(builder.Configuration.GetSection("RabbitMqConfig"));
        builder.Services.Configure<MongoDbConfig>(builder.Configuration.GetSection(nameof(MongoDbConfig)));
        builder.Services.Configure<AnomalyDetectionConfig>(builder.Configuration.GetSection(nameof(AnomalyDetectionConfig)));
        builder.Services.Configure<SignalRConfig>(builder.Configuration.GetSection(nameof(SignalRConfig)));


        builder.Services.AddSingleton<MongoDbContext>();
        builder.Services.AddTransient<IMessageBroker> (sp =>
        {
            var options = sp.GetRequiredService<IOptions<MessageBrokerConfig>>().Value;
            return new RabbitMq(options.Uri, options.ServerName, options.ExchangeName);
        });
        builder.Services.AddTransient<IServerStatisticsRepository, ServerStatisticsMongoDbRepository>();
        builder.Services.AddTransient<IAnomalyDetectionService, AnomalyDetectionService>();
        builder.Services.AddTransient<IAlertStrategy, MemoryUsageAlertStrategy>();
        builder.Services.AddTransient<IAlertStrategy, CpuUsageAnomalyAlertStrategy>();
        builder.Services.AddTransient<IAlertStrategy, CpuHighUsageAlertStrategy>();
        builder.Services.AddTransient<IAlertStrategy, MemoryHighUsageAlertStrategy>();

        builder.Services.AddTransient<AnomalyDetectionApp>();

        builder.Services.AddHostedService<AnomalyDetectionBackgroundService>();

        var app = builder.Build();
        var signalROptions = app.Services.GetRequiredService<IOptions<SignalRConfig>>().Value;
        app.MapHub<NotificationHub>($"/{signalROptions.HubUrl}");
        await app.RunAsync(signalROptions.SignalRUrl);
    }
}
