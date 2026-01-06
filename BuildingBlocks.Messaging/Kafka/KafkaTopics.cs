namespace BuildingBlocks.Messaging.Kafka;

/// <summary>
/// Well-known Kafka topic names used across services.
/// </summary>
public static class KafkaTopics
{
    /// <summary>
    /// Topic for order-related events.
    /// </summary>
    public const string Orders = "shopverse.orders";

    /// <summary>
    /// Topic for payment-related events.
    /// </summary>
    public const string Payments = "shopverse.payments";

    /// <summary>
    /// Topic for inventory-related events.
    /// </summary>
    public const string Inventory = "shopverse.inventory";
}
