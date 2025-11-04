namespace MessageProcessingAndAnomalyDetectionService.BackgroundServices;

public class AnomalyDetectionBackgroundService : BackgroundService
{
    private readonly AnomalyDetectionApp _anomalyApp;

    public AnomalyDetectionBackgroundService(AnomalyDetectionApp anomalyApp)
    {
        _anomalyApp = anomalyApp;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _anomalyApp.ConsumeAndProcessMessage();
            await Task.Delay(1000, stoppingToken);
        }
    }
}
