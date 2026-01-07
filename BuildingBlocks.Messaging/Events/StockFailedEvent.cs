namespace BuildingBlocks.Messaging.Events;

/// <summary>
/// Event published when stock reservation fails for an order.
/// Consumed by OrderService to cancel the order and PaymentService to refund.
/// </summary>
public sealed record StockFailedEvent : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    public string? CorrelationId { get; init; }

    /// <summary>
    /// The order that failed stock reservation.
    /// </summary>
    public required Guid OrderId { get; init; }

    /// <summary>
    /// Reason for the failure.
    /// </summary>
    public required string Reason { get; init; }

    /// <summary>
    /// List of items that failed to reserve.
    /// </summary>
    public List<FailedItemDto> FailedItems { get; init; } = [];
}

/// <summary>
/// Represents an item that failed stock reservation.
/// </summary>
public sealed record FailedItemDto
{
    public required string ProductId { get; init; }
    public required int RequestedQuantity { get; init; }
    public required int AvailableQuantity { get; init; }
}
