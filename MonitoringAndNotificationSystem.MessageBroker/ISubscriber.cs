namespace MonitoringAndNotificationSystem.MessageBroker;

public interface ISubscriber
{
    Task ConsumeAsync<T>(string topic, Func<T, Task> processMessage);
}