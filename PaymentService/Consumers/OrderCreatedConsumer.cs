using System.Text.Json;
using BuildingBlocks.Messaging.Events;
using BuildingBlocks.Messaging.Kafka;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace PaymentService.Consumers;

/// <summary>
/// Background service that consumes OrderCreated events from Kafka
/// and initiates payment processing.
/// </summary>
public sealed class OrderCreatedConsumer : BackgroundService
{
    private readonly ILogger<OrderCreatedConsumer> _logger;
    private readonly KafkaSettings _settings;
    private readonly JsonSerializerOptions _jsonOptions;

    public OrderCreatedConsumer(IOptions<KafkaSettings> settings, ILogger<OrderCreatedConsumer> logger)
    {
        _logger = logger;
        _settings = settings.Value;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OrderCreatedConsumer starting...");

        // Wait a bit for Kafka to be ready
        await Task.Delay(5000, stoppingToken);

        var config = new ConsumerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            GroupId = _settings.GroupId ?? "payment-service",
            AutoOffsetReset = Enum.Parse<AutoOffsetReset>(_settings.AutoOffsetReset, ignoreCase: true),
            EnableAutoCommit = _settings.EnableAutoCommit
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();

        consumer.Subscribe(KafkaTopics.Orders);
        _logger.LogInformation("Subscribed to topic: {Topic}", KafkaTopics.Orders);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(stoppingToken);

                    if (consumeResult?.Message?.Value is null)
                        continue;

                    _logger.LogInformation(
                        "Received message from {Topic} [Partition: {Partition}, Offset: {Offset}]",
                        consumeResult.Topic,
                        consumeResult.Partition.Value,
                        consumeResult.Offset.Value);

                    // Deserialize the event
                    var orderCreatedEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(
                        consumeResult.Message.Value,
                        _jsonOptions);

                    if (orderCreatedEvent is not null)
                    {
                        await ProcessOrderCreatedAsync(orderCreatedEvent, stoppingToken);
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming message");
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("OrderCreatedConsumer stopping...");
        }
        finally
        {
            consumer.Close();
        }
    }

    private Task ProcessOrderCreatedAsync(OrderCreatedEvent @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Processing payment for Order {OrderId}, User: {UserId}, Amount: {Amount} {Currency}, CorrelationId: {CorrelationId}",
            @event.OrderId,
            @event.UserId,
            @event.TotalAmount,
            @event.Currency,
            @event.CorrelationId);

        // TODO: Implement actual payment processing logic
        // - Validate payment method
        // - Charge payment gateway
        // - Publish PaymentCompleted or PaymentFailed event

        _logger.LogInformation("Payment initiated for Order {OrderId}", @event.OrderId);

        return Task.CompletedTask;
    }
}
