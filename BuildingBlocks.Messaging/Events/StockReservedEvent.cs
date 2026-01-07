namespace BuildingBlocks.Messaging.Events;

/// <summary>
/// Event published when stock is successfully reserved for an order.
/// Consumed by OrderService to confirm the order.
/// </summary>
public sealed record StockReservedEvent : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    public string? CorrelationId { get; init; }

    /// <summary>
    /// The order this reservation is for.
    /// </summary>
    public required Guid OrderId { get; init; }

    /// <summary>
    /// List of reserved items.
    /// </summary>
    public List<ReservedItemDto> ReservedItems { get; init; } = [];
}

/// <summary>
/// Represents a reserved inventory item.
/// </summary>
public sealed record ReservedItemDto
{
    public required string ProductId { get; init; }
    public required int Quantity { get; init; }
    public required string WarehouseLocation { get; init; }
}
