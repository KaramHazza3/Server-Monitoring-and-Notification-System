using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MonitoringAndNotificationSystem.MessageBroker;

public sealed class RabbitMq : IMessageBroker, IAsyncDisposable
{
    private readonly IConnectionFactory _connectionFactory;
    private IConnection? _connection;
    private IChannel? _channel;
    private string _exchangeName;
    private string? _queueName;

    public RabbitMq(string uri,
        string serverName,
        string exchangeName,
        string? queueName = null)
    {
        _connectionFactory = new ConnectionFactory();
        _connectionFactory.Uri = new Uri(uri);
        _connectionFactory.ClientProvidedName = serverName;
        _exchangeName = exchangeName;
        if (queueName is not null)
        {
            _queueName = queueName;
        }
    }
    public async Task PublishAsync<T>(string routingKey, T message)
    {
        await using var channel = await GetChannelAsync();
        await channel.ExchangeDeclareAsync(_exchangeName, ExchangeType.Topic);
        await using var messageStream = new MemoryStream();

        await JsonSerializer.SerializeAsync(messageStream, message, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });
        
        messageStream.Position = 0;
        await channel.BasicPublishAsync(_exchangeName, routingKey, false, messageStream.ToArray());
    }

    public async Task ConsumeAsync<T>(string routingKeyPattern, Func<T, Task> processMessage)
    {
        var channel = await GetChannelAsync();
        await channel.ExchangeDeclareAsync(_exchangeName, ExchangeType.Topic);
        var queueName = _queueName ?? "default-queue";
        var createdQueueName = (await channel.QueueDeclareAsync(queueName, false, true, true)).QueueName;
        await channel.QueueBindAsync(createdQueueName, _exchangeName, routingKeyPattern);
        
        await channel.BasicQosAsync(0, 1, false);
        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, args) =>
        {
            try
            {
                var body = args.Body.ToArray();
                await using var stream = new MemoryStream(body);
                var message = await JsonSerializer.DeserializeAsync<T>(stream, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });
                if (message is null)
                {
                    throw new NullReferenceException("There's an error while decoding the object");
                }
                await processMessage(message);
                await channel.BasicAckAsync(args.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while processing message: {ex.Message}");
                await channel.BasicNackAsync(args.DeliveryTag, false, true);
            }
        };
        
        await channel.BasicConsumeAsync(
            queue: createdQueueName,
            autoAck: false,
            consumer: consumer);
    }

    private async Task<IChannel> GetChannelAsync()
    {
        _connection ??= await _connectionFactory.CreateConnectionAsync();
        if (_channel is null || !_channel.IsOpen)
            _channel = await _connection.CreateChannelAsync();

        return _channel;
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null)
        {
            await _channel.DisposeAsync();
            _channel = null;
        }

        if (_connection != null)
        {
            await _connection.DisposeAsync();
            _connection = null;
        }
    }
}