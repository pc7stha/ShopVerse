namespace BuildingBlocks.Messaging.Events;

/// <summary>
/// Event published when a new order is created.
/// Consumed by PaymentService to initiate payment processing.
/// </summary>
public sealed record OrderCreatedEvent : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    public string? CorrelationId { get; init; }

    /// <summary>
    /// The unique order identifier.
    /// </summary>
    public required Guid OrderId { get; init; }

    /// <summary>
    /// The user who placed the order.
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// Total amount to be paid.
    /// </summary>
    public required decimal TotalAmount { get; init; }

    /// <summary>
    /// Currency code (e.g., USD, EUR).
    /// </summary>
    public string Currency { get; init; } = "USD";

    /// <summary>
    /// List of items in the order.
    /// </summary>
    public List<OrderItemDto> Items { get; init; } = [];
}

/// <summary>
/// Represents an item in an order.
/// </summary>
public sealed record OrderItemDto
{
    public required string ProductId { get; init; }
    public required string ProductName { get; init; }
    public required int Quantity { get; init; }
    public required decimal UnitPrice { get; init; }
}
