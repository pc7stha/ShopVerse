namespace BuildingBlocks.Messaging.Events;

/// <summary>
/// Base interface for all integration events.
/// </summary>
public interface IIntegrationEvent
{
    /// <summary>
    /// Unique identifier for this event instance.
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// Timestamp when the event occurred.
    /// </summary>
    DateTime OccurredOn { get; }

    /// <summary>
    /// Correlation ID for tracing across services.
    /// </summary>
    string? CorrelationId { get; }
}
