namespace OrderService.Models;

/// <summary>
/// Request model for creating a new order.
/// </summary>
public sealed record CreateOrderRequest
{
    /// <summary>
    /// List of items to order.
    /// </summary>
    public required List<OrderItemRequest> Items { get; init; }
}

/// <summary>
/// An item in the order request.
/// </summary>
public sealed record OrderItemRequest
{
    public required string ProductId { get; init; }
    public required string ProductName { get; init; }
    public required int Quantity { get; init; }
    public required decimal UnitPrice { get; init; }
}

/// <summary>
/// Response after creating an order.
/// </summary>
public sealed record CreateOrderResponse
{
    public required Guid OrderId { get; init; }
    public required string Status { get; init; }
    public required decimal TotalAmount { get; init; }
    public required DateTime CreatedAt { get; init; }
}
