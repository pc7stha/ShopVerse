using BuildingBlocks.Messaging.Events;

namespace BuildingBlocks.Messaging.Kafka;

/// <summary>
/// Interface for publishing events to Kafka.
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    /// Publishes an event to the specified topic.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <param name="topic">The Kafka topic name.</param>
    /// <param name="event">The event to publish.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task PublishAsync<TEvent>(string topic, TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent;
}
