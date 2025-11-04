namespace MonitoringAndNotificationSystem.MessageBroker;

public interface IPublisher
{
    Task PublishAsync<T>(string topic, T message);
}