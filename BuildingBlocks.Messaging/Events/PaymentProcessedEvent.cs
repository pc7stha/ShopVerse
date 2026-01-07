namespace BuildingBlocks.Messaging.Events;

/// <summary>
/// Event published when payment is successfully processed.
/// Consumed by OrderService to update order status and InventoryService to confirm reservation.
/// </summary>
public sealed record PaymentProcessedEvent : IIntegrationEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    public string? CorrelationId { get; init; }

    /// <summary>
    /// The payment transaction ID.
    /// </summary>
    public required Guid PaymentId { get; init; }

    /// <summary>
    /// The order this payment is for.
    /// </summary>
    public required Guid OrderId { get; init; }

    /// <summary>
    /// The user who made the payment.
    /// </summary>
    public required string UserId { get; init; }

    /// <summary>
    /// Amount paid.
    /// </summary>
    public required decimal Amount { get; init; }

    /// <summary>
    /// Currency code.
    /// </summary>
    public string Currency { get; init; } = "USD";

    /// <summary>
    /// Payment status (Completed, Failed).
    /// </summary>
    public required string Status { get; init; }

    /// <summary>
    /// Payment method used (CreditCard, PayPal, etc.).
    /// </summary>
    public string? PaymentMethod { get; init; }

    /// <summary>
    /// External transaction reference from payment gateway.
    /// </summary>
    public string? TransactionReference { get; init; }
}
