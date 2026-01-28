using OrderService.Domain.Enums;

namespace OrderService.Application.DTOs;

/// <summary>
/// Request DTO for updating an order's status.
/// Aligned with OpenAPI contract specification.
/// </summary>
public class UpdateOrderStatusRequest
{
    /// <summary>
    /// The new status to set for the order.
    /// Valid values: Pending, Confirmed, Shipped, Delivered, Cancelled
    /// </summary>
    public OrderStatus NewStatus { get; set; }
}
