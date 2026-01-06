using System.Text.Json;
using BuildingBlocks.Messaging.Events;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Messaging.Kafka;

/// <summary>
/// Kafka event publisher implementation.
/// </summary>
public sealed class KafkaEventPublisher : IEventPublisher, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaEventPublisher> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public KafkaEventPublisher(IOptions<KafkaSettings> settings, ILogger<KafkaEventPublisher> logger)
    {
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        var config = new ProducerConfig
        {
            BootstrapServers = settings.Value.BootstrapServers,
            Acks = Acks.All,
            EnableIdempotence = true,
            MessageSendMaxRetries = 3,
            RetryBackoffMs = 1000
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
        _logger.LogInformation("Kafka producer initialized with servers: {Servers}", settings.Value.BootstrapServers);
    }

    public async Task PublishAsync<TEvent>(string topic, TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent
    {
        var key = @event.EventId.ToString();
        var value = JsonSerializer.Serialize(@event, _jsonOptions);

        var message = new Message<string, string>
        {
            Key = key,
            Value = value,
            Headers = new Headers
            {
                { "event-type", System.Text.Encoding.UTF8.GetBytes(typeof(TEvent).Name) },
                { "correlation-id", System.Text.Encoding.UTF8.GetBytes(@event.CorrelationId ?? "") }
            }
        };

        try
        {
            var result = await _producer.ProduceAsync(topic, message, cancellationToken);
            _logger.LogInformation(
                "Published {EventType} to {Topic} [Partition: {Partition}, Offset: {Offset}]",
                typeof(TEvent).Name,
                topic,
                result.Partition.Value,
                result.Offset.Value);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex, "Failed to publish {EventType} to {Topic}", typeof(TEvent).Name, topic);
            throw;
        }
    }

    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(10));
        _producer.Dispose();
    }
}
