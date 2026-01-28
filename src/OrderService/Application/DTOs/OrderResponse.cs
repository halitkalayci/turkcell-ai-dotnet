using OrderService.Domain.Enums;

namespace OrderService.Application.DTOs;

/// <summary>
/// Response DTO representing an order item.
/// </summary>
public class OrderItemResponse
{
    /// <summary>
    /// Unique identifier of the order item.
    /// </summary>
    public Guid OrderItemId { get; set; }

    /// <summary>
    /// Unique identifier of the product.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Name of the product.
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Quantity of the product ordered.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Price per unit of the product.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Total price for this item (quantity * unit price).
    /// </summary>
    public decimal TotalPrice { get; set; }
}

/// <summary>
/// Response DTO representing an order with its items.
/// Aligned with OpenAPI contract specification.
/// </summary>
public class OrderResponse
{
    /// <summary>
    /// Unique identifier of the order.
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// Unique identifier of the customer who placed the order.
    /// </summary>
    public Guid CustomerId { get; set; }

    /// <summary>
    /// Date and time when the order was created.
    /// </summary>
    public DateTime OrderDate { get; set; }

    /// <summary>
    /// Current status of the order (Pending, Confirmed, Shipped, Delivered, Cancelled).
    /// </summary>
    public OrderStatus Status { get; set; }

    /// <summary>
    /// Total amount of the order.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// List of items in the order.
    /// </summary>
    public List<OrderItemResponse> Items { get; set; } = new();
}
