using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MonitoringAndNotificationSystem.MessageBroker;
using ServerStatisticsCollectionService.Config;

using ServerStatisticsCollectionService.Services.ServerStatisticsCollector;
using ServerStatisticsCollectionService.Services.ServerStatisticsCollector.Intf;

namespace ServerStatisticsCollectionService;

class Program
{
    static async Task Main(string[] args)
    {
        using IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                config.AddUserSecrets<Program>();
                config.AddEnvironmentVariables();
            })
            .ConfigureServices((context, services) =>
            {
                services.Configure<ServerStatisticsConfig>(context.Configuration.GetSection(nameof(ServerStatisticsConfig)));
                services.Configure<MessageBrokerConfig>(context.Configuration.GetSection("RabbitMqConfig"));
                services.AddTransient<ServerStatisticsPublishApp>();
                services.AddTransient<ServerStatisticsCollector>();
                services.AddTransient<IMessageBroker> (sp =>
                {
                    var options = sp.GetRequiredService<IOptions<MessageBrokerConfig>>().Value;
                    return new RabbitMq(options.Uri, options.ServerName, options.ExchangeName);
                });
                services.AddTransient<IServerStatisticsCollector, ServerStatisticsCollector>();
            })
            .Build();

        var app = host.Services.GetRequiredService<ServerStatisticsPublishApp>();
        while (true)
        {
            await app.CollectAndPublishAsync();
        }
    }
}