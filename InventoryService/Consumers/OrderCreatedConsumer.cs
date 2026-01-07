using System.Text.Json;
using BuildingBlocks.Messaging.Events;
using BuildingBlocks.Messaging.Kafka;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace InventoryService.Consumers;

/// <summary>
/// Background service that consumes OrderCreated events from Kafka
/// and reserves stock for the order.
/// </summary>
public sealed class OrderCreatedConsumer : BackgroundService
{
    private readonly ILogger<OrderCreatedConsumer> _logger;
    private readonly KafkaSettings _settings;
    private readonly IEventPublisher _eventPublisher;
    private readonly JsonSerializerOptions _jsonOptions;

    // Simulated inventory (in production, this would be a database)
    private static readonly Dictionary<string, int> _inventory = new()
    {
        { "laptop-001", 100 },
        { "mouse-001", 500 },
        { "keyboard-001", 200 },
        { "monitor-001", 50 },
        { "headphones-001", 150 }
    };

    public OrderCreatedConsumer(
        IOptions<KafkaSettings> settings,
        IEventPublisher eventPublisher,
        ILogger<OrderCreatedConsumer> logger)
    {
        _logger = logger;
        _settings = settings.Value;
        _eventPublisher = eventPublisher;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("InventoryService OrderCreatedConsumer starting...");

        // Wait a bit for Kafka to be ready
        await Task.Delay(5000, stoppingToken);

        var config = new ConsumerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            GroupId = _settings.GroupId ?? "inventory-service",
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
            _logger.LogInformation("InventoryService OrderCreatedConsumer stopping...");
        }
        finally
        {
            consumer.Close();
        }
    }

    private async Task ProcessOrderCreatedAsync(OrderCreatedEvent @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Checking stock for Order {OrderId}, Items: {ItemCount}, CorrelationId: {CorrelationId}",
            @event.OrderId,
            @event.Items.Count,
            @event.CorrelationId);

        var failedItems = new List<FailedItemDto>();
        var reservedItems = new List<ReservedItemDto>();

        foreach (var item in @event.Items)
        {
            var available = _inventory.GetValueOrDefault(item.ProductId, 0);

            if (available >= item.Quantity)
            {
                // Reserve the stock
                _inventory[item.ProductId] = available - item.Quantity;
                reservedItems.Add(new ReservedItemDto
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    WarehouseLocation = "Warehouse-A"
                });

                _logger.LogInformation(
                    "Reserved {Quantity} of {ProductId} for Order {OrderId}. Remaining: {Remaining}",
                    item.Quantity, item.ProductId, @event.OrderId, _inventory[item.ProductId]);
            }
            else
            {
                failedItems.Add(new FailedItemDto
                {
                    ProductId = item.ProductId,
                    RequestedQuantity = item.Quantity,
                    AvailableQuantity = available
                });

                _logger.LogWarning(
                    "Insufficient stock for {ProductId}. Requested: {Requested}, Available: {Available}",
                    item.ProductId, item.Quantity, available);
            }
        }

        if (failedItems.Count == 0)
        {
            // All items reserved successfully
            var stockReservedEvent = new StockReservedEvent
            {
                OrderId = @event.OrderId,
                ReservedItems = reservedItems,
                CorrelationId = @event.CorrelationId
            };

            await _eventPublisher.PublishAsync(KafkaTopics.Inventory, stockReservedEvent, cancellationToken);
            _logger.LogInformation("Published StockReservedEvent for Order {OrderId}", @event.OrderId);
        }
        else
        {
            // Some items failed - rollback any reservations made
            foreach (var reserved in reservedItems)
            {
                _inventory[reserved.ProductId] += reserved.Quantity;
            }

            var stockFailedEvent = new StockFailedEvent
            {
                OrderId = @event.OrderId,
                Reason = "Insufficient stock for one or more items",
                FailedItems = failedItems,
                CorrelationId = @event.CorrelationId
            };

            await _eventPublisher.PublishAsync(KafkaTopics.Inventory, stockFailedEvent, cancellationToken);
            _logger.LogInformation("Published StockFailedEvent for Order {OrderId}", @event.OrderId);
        }
    }
}
