using System.Security.Claims;
using BuildingBlocks.Messaging.Events;
using BuildingBlocks.Messaging.Kafka;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Models;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IEventPublisher eventPublisher, ILogger<OrdersController> logger)
    {
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get() => Ok("Order Service Works!");

    /// <summary>
    /// Creates a new order and publishes an OrderCreated event.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        // Get user ID from JWT claims
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                     ?? User.FindFirstValue("sub") 
                     ?? "anonymous";

        var orderId = Guid.NewGuid();
        var totalAmount = request.Items.Sum(i => i.Quantity * i.UnitPrice);
        var correlationId = HttpContext.TraceIdentifier;

        _logger.LogInformation(
            "Creating order {OrderId} for user {UserId} with {ItemCount} items, total: {Total}",
            orderId, userId, request.Items.Count, totalAmount);

        // Create the event
        var orderCreatedEvent = new OrderCreatedEvent
        {
            OrderId = orderId,
            UserId = userId,
            TotalAmount = totalAmount,
            CorrelationId = correlationId,
            Items = request.Items.Select(i => new OrderItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        // Publish to Kafka
        await _eventPublisher.PublishAsync(KafkaTopics.Orders, orderCreatedEvent, cancellationToken);

        _logger.LogInformation("Published OrderCreated event for order {OrderId}", orderId);

        var response = new CreateOrderResponse
        {
            OrderId = orderId,
            Status = "Created",
            TotalAmount = totalAmount,
            CreatedAt = orderCreatedEvent.OccurredOn
        };

        return CreatedAtAction(nameof(GetOrder), new { id = orderId }, response);
    }

    /// <summary>
    /// Gets an order by ID (placeholder).
    /// </summary>
    [HttpGet("{id:guid}")]
    public IActionResult GetOrder(Guid id)
    {
        // TODO: Implement order retrieval from database
        return Ok(new { OrderId = id, Status = "Pending" });
    }
}
