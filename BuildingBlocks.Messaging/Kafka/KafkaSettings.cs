namespace BuildingBlocks.Messaging.Kafka;

/// <summary>
/// Kafka configuration settings.
/// </summary>
public sealed class KafkaSettings
{
    public const string SectionName = "Kafka";

    /// <summary>
    /// Comma-separated list of Kafka broker addresses.
    /// </summary>
    public string BootstrapServers { get; set; } = "localhost:19092";

    /// <summary>
    /// Consumer group ID for this service.
    /// </summary>
    public string? GroupId { get; set; }

    /// <summary>
    /// Enable auto-commit for consumer offsets.
    /// </summary>
    public bool EnableAutoCommit { get; set; } = true;

    /// <summary>
    /// Auto offset reset behavior (earliest, latest).
    /// </summary>
    public string AutoOffsetReset { get; set; } = "earliest";
}
